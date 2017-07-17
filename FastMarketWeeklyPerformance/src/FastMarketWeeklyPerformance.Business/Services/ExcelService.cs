using System.IO;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Repositories;
using OfficeOpenXml;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export();

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly DataSourceService dataSourceService;
        private readonly DateRangeService dateRangeService;
        private readonly ApplicationConfigurationService appConfigService;
        private readonly ExcelPackage excelPackage;

        private DateRange dateRange;
        private AppConfig appConfig;

        public ExcelServiceImp(DataSourceService dataSourceService, DateRangeService dateRangeService, ApplicationConfigurationService appConfigService, ExcelPackage excelPackage)
        {
            this.dataSourceService = dataSourceService;
            this.dateRangeService = dateRangeService;
            this.appConfigService = appConfigService;
            this.excelPackage = excelPackage;
        }

        public ExcelService Export()
        {
            dateRange = dateRangeService.DateRange(appConfigService.Configurations.ReportDateRange);
            appConfig = appConfigService.Configurations;

            var dataSource = dataSourceService.Query(new FastMarketByWeeklyCriteria { FromDate = dateRange.FromDate, ToDate = dateRange.ToDate });
            var rowCount = dataSource.Rows.Count;
            var columnCount = dataSource.Columns.Count;
            var excelWorksheet = excelPackage.Workbook.Worksheets["Fast Market"];
            excelWorksheet.Cells["B8"].LoadFromDataTable(dataSource, false);

            FormatHeader(excelWorksheet);
            FormatContent(excelWorksheet, rowCount, columnCount);

            return this;
        }

        private void FormatHeader(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells["B2"].Value = excelWorksheet.Cells["B2"].Value
                .ToString()
                .Replace("{0}", $"{dateRange.FromDate:dd/MM/yyyyy}")
                .Replace("{1}", $"{dateRange.ToDate:dd/MM/yyyyy}");

            excelWorksheet.Cells["B3"].Value = excelWorksheet.Cells["B3"].Value
                .ToString()
                .Replace("{0}", $"{SystemTime.Now}");
        }

        private static void FormatContent(ExcelWorksheet excelWorksheet, int rowCount, int columnCount)
        {
            var startRowIndex = 8;
            var endRowIndex = rowCount + startRowIndex - 1;
            var startColumnIndex = 2;
            var endColumnIndex = startColumnIndex + columnCount;
            for (var columnIndex = startColumnIndex; columnIndex < endColumnIndex; columnIndex++)
            {
                excelWorksheet.Cells[startRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelWorksheet.Cells[startRowIndex, columnIndex].StyleID;
            }
        }

        public string Save()
        {
            var fileName = $"{dateRange.FromDate:yyyyMMdd}_{dateRange.ToDate:yyyyMMdd}_Automail_FastMarket_WeeklyPerformance.xlsx";
            var path = $@"{appConfig.ExportDataDirectory}\{SystemTime.Now:yyyy}\{SystemTime.Now:MM}";
            Directory.CreateDirectory(path);
            var fullPath = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(fullPath), appConfig.ExcelPasswordProtected);
            return fullPath;
        }
    }
}