using System.IO;
using Fanex.Service.Core;
using OfficeOpenXml;
using TopWinningAgent.Business.Repositories;

namespace TopWinningAgent.Business.Services
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

        public ExcelServiceImp(DataSourceService dataSourceService, DateRangeService dateRangeService, ApplicationConfigurationService appConfigService, ExcelPackage excelPackage)
        {
            this.dataSourceService = dataSourceService;
            this.dateRangeService = dateRangeService;
            this.appConfigService = appConfigService;
            this.excelPackage = excelPackage;
        }

        public ExcelService Export()
        {
            var dataSource = dataSourceService.Query(new Criteria());
            var rowCount = dataSource.Rows.Count;
            var columnCount = dataSource.Columns.Count;
            var excelWorksheet = excelPackage.Workbook.Worksheets["TOP WINNING AGENT"];
            excelWorksheet.Cells["B8"].LoadFromDataTable(dataSource, false);

            FormatHeader(excelWorksheet);
            FormatContent(excelWorksheet, rowCount, columnCount);

            return this;
        }

        private void FormatHeader(ExcelWorksheet excelWorksheet)
        {            
            var title = $"{dateRangeService.FromDate(1): MMM dd, yyyy} - {dateRangeService.ToDate(): MMM dd, yyyy}";
            var headerLastWeek = $"Last Week ({dateRangeService.FromDate(1):MMM dd, yyyy} - {dateRangeService.ToDate():MMM dd, yyyy})";
            var headerLast4Weeks = $"Last 4 Weeks ({dateRangeService.FromDate(4):MMM dd, yyyy} - {dateRangeService.ToDate():MMM dd, yyyy})";
            var headerLast12Weeks = $"Last 12 Weeks ({dateRangeService.FromDate(12):MMM dd, yyyy} - {dateRangeService.ToDate():MMM dd, yyyy})";

            excelWorksheet.Cells[3, 1].Value = title;
            excelWorksheet.Cells[6, 4].Value = headerLastWeek;
            excelWorksheet.Cells[6, 9].Value = headerLast4Weeks;
            excelWorksheet.Cells[6, 14].Value = headerLast12Weeks;
        }

        private static void FormatContent(ExcelWorksheet excelWorksheet, int rowCount, int columnCount)
        {
            var startRowIndex = 8;
            var endRowIndex = rowCount + startRowIndex - 1;
            var startColumnIndex = 1;
            var endColumnIndex = startColumnIndex + columnCount;
            for (var columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
            {
                excelWorksheet.Cells[startRowIndex, columnIndex, endRowIndex, columnIndex].StyleID = excelWorksheet.Cells[startRowIndex, columnIndex].StyleID;
            }

            for (var index = 0; index < rowCount; index++)
            {
                excelWorksheet.Cells[startRowIndex + index, 1].Value = index + 1;
            }
        }

        public string Save()
        {
            var fileName = $"Weekly_Top_Winning_Agent_From_{dateRangeService.FromDate(1):yyyyMMdd}_To_{dateRangeService.ToDate():yyyyMMdd}.xlsx";
            var path = $@"{appConfigService.Configurations.ExportDataDirectory}\{SystemTime.Now:yyyy}\{SystemTime.Now:MM}";
            Directory.CreateDirectory(path);
            var fullPath = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(fullPath), appConfigService.Configurations.ExcelPasswordProtected);
            return fullPath;
        }
    }
}