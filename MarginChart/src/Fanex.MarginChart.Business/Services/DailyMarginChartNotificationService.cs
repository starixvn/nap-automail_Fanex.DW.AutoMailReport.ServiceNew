using Fanex.MailDelivery.Core.Models;
using Fanex.MarginChart.Business.Models;
using Fanex.MarginChart.Business.Services.Margin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.MarginChart.Business.Services
{
    public interface DailyMarginChartNotificationService
    {
        JobMessage Execute();
    }

    public class DailyMarginChartNotificationServiceImp : DailyMarginChartNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelRawService excelRawService;
        private readonly MessageService messageService;
        private readonly FileService fileService;
        private readonly BaseDataTable baseDataTable;
        private readonly ImageMarginService imageMarginService;
        private readonly DateTimeServiceProvider dateTimeServiceProvider;
        private readonly DataMarginService dataMarginService;
        private readonly ExcelMarginService excelMarginService;
        private readonly ChartService chartService;

        public DailyMarginChartNotificationServiceImp(
            Log log,
            DataService dataService,
            ExcelRawService excelRawService,
            MessageService messageService,
            FileService fileService,
            BaseDataTable baseDataTable,
            ImageMarginService imageMarginService,
            DateTimeServiceProvider dateTimeServiceProvider,
            DataMarginService dataMarginService,
            ExcelMarginService excelMarginService,
            ChartService chartService)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelRawService = excelRawService;
            this.messageService = messageService;
            this.fileService = fileService;
            this.baseDataTable = baseDataTable;
            this.imageMarginService = imageMarginService;
            this.dateTimeServiceProvider = dateTimeServiceProvider;
            this.dataMarginService = dataMarginService;
            this.excelMarginService = excelMarginService;
            this.chartService = chartService;
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new JobMessage();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.SuccessWithoutSendmail : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            if (!dataService.IsDataAvailable())
            {
                return new ExecuteResultModel
                {
                    Mail = new Mail(),
                    Message = "DataIsNotReady, TaskID = 77",
                    Success = false
                };
            }

            var fullRawData = dataService.QueryMarginChartData();
            var dailyRawData = dataService.ThousandScale(fullRawData, 10);

            var json = JsonConvert.SerializeObject(dailyRawData, Formatting.Indented);

            Directory.CreateDirectory(AppConfigService.Config.Path);
            var fileName = $"{AppConfigService.Config.Path}/MarginChartData.json";
            File.WriteAllText(fileName, json);

            var source = baseDataTable.CloneWithoutIgnoreFigureRows(dailyRawData);
            var fromdate = dateTimeServiceProvider.DateTime.FromDate;
            var todate = dateTimeServiceProvider.DateTime.ToDate;
            var products = AppConfigService.Config.ProductGroups;
            var yesterdayColumnIndex = source.Columns.Count - 1;
            var mtdColumnIndex = ColumnIndex.MTDColumnIndex;
            var statusSendImage = new Dictionary<string, bool>();

            excelRawService.Export(dailyRawData, true, fromdate, todate);

            products.ForEach(product =>
            {
                var images = new List<string>();
                var category = product.ProductCategory;
                var reportId = product.ReportID;
                var reportIndex = 1;

                var margins = ExportMargin(source, yesterdayColumnIndex, mtdColumnIndex, category, reportId,
                    reportIndex);
                var charts = ExportChart(product, source, category);
                images.AddRange(margins);
                images.AddRange(charts);

                var status = CopyFiles(product.LetstalkGroup, images);
                status
                    .ToList()
                    .ForEach(s => { statusSendImage.Add(s.Key, s.Value); });
            });

            var errFile = statusSendImage.Where(s => !s.Value).Select(s => s.Key).ToList();

            return new ExecuteResultModel
            {
                Mail = new Mail(),
                Message = string.Join(Environment.NewLine, errFile),
                Success = errFile.Count == 0
            };
        }

        private List<string> ExportMargin(DataTable source, int yesterdayColumnIndex, int mtdColumnIndex,
            string category, string reportId, int reportIndex)
        {
            var dailyData = dataMarginService.CalcDataByCategory(source, category, yesterdayColumnIndex);
            var mtdData = dataMarginService.CalcDataByCategory(source, category, mtdColumnIndex);
            var excelDaily = excelMarginService.Export(dailyData, true, category, reportId, reportIndex);
            reportIndex++;

            var excelMtd = excelMarginService.Export(mtdData, false, category, reportId, reportIndex);
            var imgDaily = imageMarginService.Export(excelDaily, true, category, excelMarginService.LastRowIndex);
            var imgMTD = imageMarginService.Export(excelMtd, false, category, excelMarginService.LastRowIndex);

            return new List<string>
            {
                imgDaily,
                imgMTD
            };
        }

        private List<string> ExportChart(ProductGroup product, DataTable source, string category)
        {
            var dataWLForCreditChart = dataMarginService.CalcWinLossForChart(category, source, DataType.Credit);
            var dataTOForCreditChart = dataMarginService.CalcTurnoverForChart(category, source, DataType.Credit);
            var creditChart =
                chartService.ExportDailyChart(product, "Credit", dataWLForCreditChart, dataTOForCreditChart);

            var dataWLForLicenseeChart = dataMarginService.CalcWinLossForChart(category, source, DataType.Licensee);
            var dataTOForLicenseeChart = dataMarginService.CalcTurnoverForChart(category, source, DataType.Licensee);
            var licenseeChart =
                chartService.ExportDailyChart(product, "Licensee", dataWLForLicenseeChart, dataTOForLicenseeChart);

            var dataWLForAllChart = dataMarginService.CalcWinLossForChart(category, source, DataType.All);
            var dataTOForAllChart = dataMarginService.CalcTurnoverForChart(category, source, DataType.All);
            var allChart = chartService.ExportDailyChart(product, "All", dataWLForAllChart, dataTOForAllChart);

            return new List<string>
            {
                creditChart,
                licenseeChart,
                allChart
            };
        }

        private Dictionary<string, bool> CopyFiles(string letstalkGroup, List<string> images)
        {
            var result = new Dictionary<string, bool>();
            fileService.CopyFiles(letstalkGroup, images);
            if (AppConfigService.Config.IsSentDirectly)
            {
                result = messageService.SendMessage(images, letstalkGroup);
            }

            return result;
        }
    }
}