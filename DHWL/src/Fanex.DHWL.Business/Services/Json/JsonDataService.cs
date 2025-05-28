using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Models.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services.Json
{
    public interface JsonDataService
    {
        void Execute(DateTime wlDate, DataTable dataTable);
    }

    public class JsonDataServiceImp : JsonDataService
    {
        public void Execute(DateTime wlDate, DataTable dataTable)
        {
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

        private static string WriteFile(DateTime wlDate, EventDateModel eventDate)
        {
            var path = $"{AppConfigService.Config.Path}/{DateTime.Now:yyyyMMdd}";
            Directory.CreateDirectory(path);
            var fileName = $"{path}/Perf-report-{wlDate:yyyy-MM-dd}.json";
            var json = JsonConvert.SerializeObject(eventDate, Formatting.Indented);
            File.WriteAllText(fileName, json);
            return fileName;
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
                HouseTo = credit.HouseTo + licensee.HouseTo,
                HouseWl = credit.HouseWl + licensee.HouseWl,
                Credit = credit,
                Licensee = licensee,
                MtdHouseTo = mtdCredit.HouseTo + mtdLicensee.HouseTo,
                MtdHouseWl = mtdCredit.HouseWl + mtdLicensee.HouseWl,
                MtdCredit = mtdCredit,
                MtdLicensee = mtdLicensee,
            };
            return category;
        }

        private static TowlModel GetCredit(DataTable siteGroupDataTable, int lastColumn)
        {
            var credit = new TowlModel
            {
                HouseTo = siteGroupDataTable.AsEnumerable()
                    .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.HouseTurnOver))
                    .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn])),
                HouseWl = siteGroupDataTable.AsEnumerable()
                    .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.CreditHouseIncome))
                    .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn]))
            };

            return credit;
        }

        private static TowlModel GetLicensee(DataTable siteGroupDataTable, int lastColumn)
        {
            var licensee = new TowlModel
            {
                HouseTo = siteGroupDataTable.AsEnumerable()
                    .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.HouseTurnOver))
                    .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn])),
                HouseWl = siteGroupDataTable.AsEnumerable()
                    .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.LicenseeHouseIncome))
                    .Sum(s => s[lastColumn] is DBNull ? 0 : Convert.ToDecimal(s[lastColumn]))
            };

            return licensee;
        }

        private static TowlModel GetMtdCredit(DataTable siteGroupDataTable)
        {
            var credit = new TowlModel
            {
                HouseTo = siteGroupDataTable.AsEnumerable()
                    .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.HouseTurnOver))
                    .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"])),
                HouseWl = siteGroupDataTable.AsEnumerable()
                    .Where(s => !Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.CreditHouseIncome))
                    .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"]))
            };

            return credit;
        }

        private static TowlModel GetMtdLicensee(DataTable siteGroupDataTable)
        {
            var licensee = new TowlModel
            {
                HouseTo = siteGroupDataTable.AsEnumerable()
                    .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.HouseTurnOver))
                    .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"])),
                HouseWl = siteGroupDataTable.AsEnumerable()
                    .Where(s => Convert.ToBoolean(s["isDeposit"]) &&
                                s["Figure"].Equals(RowFigureName.LicenseeHouseIncome))
                    .Sum(s => s["MTD"] is DBNull ? 0 : Convert.ToDecimal(s["MTD"]))
            };

            return licensee;
        }
    }
}