using Fanex.Data;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Repositories;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export();

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly DateRangeService dateRangeService;
        private readonly ApplicationConfigurationService appConfigService;
        private readonly ExcelPackage excelPackage;

        private DateRange dateRange;
        private AppConfig appConfig;

        public ExcelServiceImp(DateRangeService dateRangeService,
                               ApplicationConfigurationService appConfigService,
                               ExcelPackage excelPackage)
        {
            this.dateRangeService = dateRangeService;
            this.appConfigService = appConfigService;
            this.excelPackage = excelPackage;
        }

        public ExcelService Export()
        {
            dateRange = dateRangeService.DateRange(appConfigService.Configurations.ReportDateRange);
            appConfig = appConfigService.Configurations;

            var criteria = new FastMarketByWeeklyCriteria { FromDate = dateRange.FromDate, ToDate = dateRange.ToDate };
            var dataSource = QueryData(criteria);
            var rowCount = dataSource.Rows.Count;
            var columnCount = dataSource.Columns.Count;
            var excelWorksheet = excelPackage.Workbook.Worksheets["Fast Market"];
            excelWorksheet.Cells["B8"].LoadFromDataTable(dataSource, false);

            FormatHeader(excelWorksheet);
            FormatContent(excelWorksheet, rowCount, columnCount);

            return this;
        }

        private DataTable QueryData(FastMarketByWeeklyCriteria criteria)
        {
            DataTable dataTable = new DataTable();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey(), 0))
            {
                db.SetParameterValue("fromdate", criteria.FromDate);
                db.SetParameterValue("todate", criteria.ToDate);
                var ds = db.ExecuteDataSet();
                if (ds.Tables.Count > 0)
                {
                    dataTable = ds.Tables[0];
                }
            }

            return dataTable;
        }

        private void FormatHeader(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells["B2"].Value = excelWorksheet.Cells["B2"].Value
                .ToString()
                .Replace("{0}", $"{dateRange.FromDate:dd/MM/yyyyy}")
                .Replace("{1}", $"{dateRange.ToDate:dd/MM/yyyyy}");

            excelWorksheet.Cells["B3"].Value = excelWorksheet.Cells["B3"].Value
                .ToString()
                .Replace("{0}", $"{DateTime.Now}");
        }

        private static void FormatContent(ExcelWorksheet excelWorksheet, int rowCount, int columnCount)
        {
            const int StartRowIndex = 8;
            var endRowIndex = rowCount + StartRowIndex - 1;
            const int StartColumnIndex = 2;
            var endColumnIndex = StartColumnIndex + columnCount;
            for (var columnIndex = StartColumnIndex; columnIndex < endColumnIndex; columnIndex++)
            {
                excelWorksheet.Cells[StartRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelWorksheet.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        public string Save()
        {
            var fileName = $"{dateRange.FromDate:yyyyMMdd}_{dateRange.ToDate:yyyyMMdd}_Automail_FastMarket_WeeklyPerformance.xlsx";
            var path = $@"{appConfig.ExportDataDirectory}\{DateTime.Now:yyyy}\{DateTime.Now:MM}";
            Directory.CreateDirectory(path);
            var fullPath = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(fullPath), appConfig.ExcelPasswordProtected);
            return fullPath;
        }
    }
}