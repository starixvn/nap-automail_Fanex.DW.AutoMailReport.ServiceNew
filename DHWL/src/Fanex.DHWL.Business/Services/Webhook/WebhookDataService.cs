using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Models.Webhook;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.DHWL.Business.Services.Webhook
{
    public interface WebhookDataService
    {
        void Execute(DateTime wlDate, DataTable dataTable);

        void ReaDataFromExcel();
    }

    public class WebhookDataServiceImp : WebhookDataService
    {
        private readonly string webhookUrl = AppConfigService.Config.Webhook.WebhookUrl;
        private readonly string webhookHeaderKey = AppConfigService.Config.Webhook.WebhookHeaderKey;
        private readonly string webhookToken = AppConfigService.Config.Webhook.WebhookToken;

        private static readonly HttpClient HttpClient;
        private readonly Log log;

        static WebhookDataServiceImp()
        {
            HttpClient client;

            if (AppConfigService.Config.Webhook.UseProxy)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri(AppConfigService.Config.Webhook.ProxyUrl),
                    UseDefaultCredentials = true
                };

                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };

                client = new HttpClient(httpClientHandler);
            }
            else
            {
                client = new HttpClient();
            }

            client.Timeout = TimeSpan.FromSeconds(120);
            HttpClient = client;
        }

        public WebhookDataServiceImp(Log log)
        {
            this.log = log;
        }

        private async Task<bool> SendToWebhook(EventDateModel eventDateModel)
        {
            try
            {
                if (!AppConfigService.Config.Webhook.IsEnabled)
                {
                    return true;
                }

                var requestData = new DailyPerfReportMtdModel
                {
                    Type = "daily-perf-reports-mtd",
                    Data = new List<EventDateModel> { eventDateModel }
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Post, webhookUrl))
                {
                    request.Headers.Add(webhookHeaderKey, webhookToken);
                    request.Content = content;

                    var response = await HttpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        log.Log($"Webhook request failed. Status: {response.StatusCode}, Content: {errorContent}");
                        return false;
                    }

                    await LogData(requestData, response);

                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                log.Log($"Network error while sending webhook: {ex.Message}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                log.Log($"Request timeout while sending webhook: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                log.Log($"Unexpected error while sending webhook: {ex.Message}");
                return false;
            }
        }

        private static async Task LogData(DailyPerfReportMtdModel requestData, HttpResponseMessage response)
        {
            var logData = new List<object>
            {
                requestData,
                await response.Content.ReadAsStringAsync()
            };
            var jsonLog = JsonConvert.SerializeObject(logData, Formatting.Indented);
            var folderPath = $"{AppConfigService.Config.Path}/{DateTime.Now:yyyyMMdd}";
            Directory.CreateDirectory(folderPath);
            File.WriteAllText($"{folderPath}/WebhookLog-{DateTime.Now:yyyyMMdd-HHmmss}.json", jsonLog);
        }

        public void Execute(DateTime wlDate, DataTable dataTable)
        {
            //var refineDataTable = dataTable.AsEnumerable()
            //    .Where(row =>
            //        row["Figure"].ToString() == RowFigureName.HouseTurnOver && Convert.ToDecimal(row["MTD"]) > 10000)
            //    .CopyToDataTable();

            var lastColumn = dataTable.Columns.Count - 1;
            var eventDate = new EventDateModel
            {
                Date = wlDate,
                Data = new List<TypeModel>()
            };

            GetSiteGroup(dataTable, lastColumn, eventDate);
            GetProductGroup(dataTable, lastColumn, eventDate);
            GetProductShortName(dataTable, lastColumn, eventDate);
            WriteFile(wlDate, eventDate);

            SendToWebhook(eventDate).GetAwaiter().GetResult();
        }

        private static void GetSiteGroup(DataTable dataTable, int lastColumn, EventDateModel eventDate)
        {
            var siteGroup = GetType(dataTable, "SiteGroup", lastColumn);
            eventDate.Data.Add(new TypeModel
            {
                Type = "SiteGroup",
                Categories = siteGroup
            });
        }

        private static void GetProductGroup(DataTable dataTable, int lastColumn, EventDateModel eventDate)
        {
            var siteGroup = GetType(dataTable, "ProductGroup", lastColumn);
            eventDate.Data.Add(new TypeModel
            {
                Type = "ProductGroup",
                Categories = siteGroup
            });
        }

        private static void GetProductShortName(DataTable dataTable, int lastColumn, EventDateModel eventDate)
        {
            var siteGroup = GetType(dataTable, "ProductShortName", lastColumn);
            eventDate.Data.Add(new TypeModel
            {
                Type = "ProductShortName",
                Categories = siteGroup
            });
        }

        private static void WriteFile(DateTime wlDate, EventDateModel eventDate)
        {
            var path = $"{AppConfigService.Config.Path}/{DateTime.Now:yyyyMMdd}";
            Directory.CreateDirectory(path);
            var fileName = $"{path}/Perf-report-{wlDate:yyyy-MM-dd}.json";
            var json = JsonConvert.SerializeObject(eventDate);
            File.WriteAllText(fileName, json);
        }

        private static List<CategoryModel> GetType(DataTable dataTable, string columnName, int lastColumn)
        {
            var categories = new List<CategoryModel>();
            dataTable.AsEnumerable().GroupBy(g => g[columnName]).ToList()
                .ForEach(group =>
                {
                    var siteGroup = group.Key.ToString();
                    var category = GetCategory(group, lastColumn, siteGroup);
                    categories.Add(category);
                });
            return categories;
        }

        private static CategoryModel GetCategory(IGrouping<object, DataRow> group, int lastColumn, string siteGroup)
        {
            var siteGroupDataTable = group.CopyToDataTable();
            var credit = GetCredit(siteGroupDataTable, lastColumn);
            var licensee = GetLicensee(siteGroupDataTable, lastColumn);
            var mtdCredit = GetMtdCredit(siteGroupDataTable);
            var mtdLicensee = GetMtdLicensee(siteGroupDataTable);
            var category = new CategoryModel
            {
                Category = siteGroup,
                DailyPerf = new PerfModel
                {
                    Credit = credit,
                    Licensee = licensee
                },
                MtdPerf = new PerfModel
                {
                    Credit = mtdCredit,
                    Licensee = mtdLicensee
                }
            };
            return category;
        }

        private static TowlModel GetCredit(DataTable siteGroupDataTable, int lastColumn)
        {
            var houseTo = siteGroupDataTable.AsEnumerable()
                .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.HouseTurnOver))
                .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn]));

            var houseWl = siteGroupDataTable.AsEnumerable()
                .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.CreditHouseIncome))
                .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn]));

            var credit = new TowlModel
            {
                HouseTo = Convert.ToInt32(houseTo / 1000),
                HouseWl = Convert.ToInt32(houseWl / 1000)
            };

            return credit;
        }

        private static TowlModel GetLicensee(DataTable siteGroupDataTable, int lastColumn)
        {
            var houseTo = siteGroupDataTable.AsEnumerable()
                .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.HouseTurnOver))
                .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn]));

            var houseWl = siteGroupDataTable.AsEnumerable()
                .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.LicenseeHouseIncome))
                .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn]));
            var licensee = new TowlModel
            {
                HouseTo = Convert.ToInt32(houseTo / 1000),
                HouseWl = Convert.ToInt32(houseWl / 1000)
            };

            return licensee;
        }

        private static TowlModel GetMtdCredit(DataTable siteGroupDataTable)
        {
            var houseTo = siteGroupDataTable.AsEnumerable()
                .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.HouseTurnOver))
                .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"]));

            var houseWl = siteGroupDataTable.AsEnumerable()
                .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.CreditHouseIncome))
                .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"]));

            var credit = new TowlModel
            {
                HouseTo = Convert.ToInt32(houseTo / 1000),
                HouseWl = Convert.ToInt32(houseWl / 1000)
            };

            return credit;
        }

        private static TowlModel GetMtdLicensee(DataTable siteGroupDataTable)
        {
            var houseTo = siteGroupDataTable.AsEnumerable()
                .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.HouseTurnOver))
                .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"]));

            var houseWl = siteGroupDataTable.AsEnumerable()
                .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                            s["Figure"].Equals(RowFigureName.LicenseeHouseIncome))
                .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"]));
            var licensee = new TowlModel
            {
                HouseTo = Convert.ToInt32(houseTo / 1000),
                HouseWl = Convert.ToInt32(houseWl / 1000)
            };

            return licensee;
        }

        private DataTable ReadDataFromExcel(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    log.Log($"Excel file not found: {filePath}");
                    return null;
                }

                var dataTable = new DataTable();
                using (var package = new ExcelPackage(new FileInfo(filePath), "Star168"))
                {
                    var worksheet = package.Workbook.Worksheets["DHWL-RawData"];
                    if (worksheet == null)
                    {
                        log.Log("Sheet 'DHWL-RawData' not found in Excel file");
                        return null;
                    }

                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (var col = 2; col <= colCount; col++)
                    {
                        var columnName = worksheet.Cells[5, col].Value?.ToString() ?? $"Column{col}";
                        var cellValue = worksheet.Cells[6, col].Value;

                        if (cellValue == null)
                        {
                            dataTable.Columns.Add(columnName, typeof(string));
                        }
                        else
                        {
                            switch (cellValue.GetType().Name)
                            {
                                case "Boolean":
                                    dataTable.Columns.Add(columnName, typeof(bool));
                                    break;

                                case "Double":
                                case "Decimal":
                                    dataTable.Columns.Add(columnName, typeof(decimal));
                                    break;

                                case "DateTime":
                                    dataTable.Columns.Add(columnName, typeof(DateTime));
                                    break;

                                case "Int32":
                                case "Int64":
                                    dataTable.Columns.Add(columnName, typeof(int));
                                    break;

                                default:
                                    dataTable.Columns.Add(columnName, typeof(string));
                                    break;
                            }
                        }
                    }

                    for (var row = 6; row <= rowCount; row++)
                    {
                        var dataRow = dataTable.NewRow();
                        for (var col = 2; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value;
                            dataRow[col - 2] = cellValue ?? DBNull.Value;
                        }

                        dataTable.Rows.Add(dataRow);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                log.Log($"Error reading Excel file: {ex.Message}");
                return null;
            }
        }

        public void ReaDataFromExcel()
        {
            const string parentFolder = "D:/tmp-Data/DHWL-PROD";
            var folders = Directory.GetDirectories(parentFolder);
            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder, "*.xlsx", SearchOption.AllDirectories);
                foreach (var filePath in files)
                {
                    var dataTable = ReadDataFromExcel(filePath);
                    var strDate = filePath.Split('-')[5].Split('.')[0].Trim();
                    var dateTime = DateTime.ParseExact(strDate, "yyyyMMdd", null);

                    Execute(dateTime, dataTable);
                    Console.WriteLine($"Processed file: {filePath} for date: {dateTime:yyyy-MM-dd}");
                }
            }
        }
    }
}