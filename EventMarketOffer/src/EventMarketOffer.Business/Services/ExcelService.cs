using System.Data;
using System.IO;
using System.Linq;
using Fanex.Common;
using OfficeOpenXml;

namespace EventMarketOffer.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export();

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly AppConfigurationService appConfigService;
        private readonly DateService dateService;
        private readonly DataService dataService;

        public ExcelServiceImp(ExcelPackage excelPackage, AppConfigurationService appConfigService, DateService dateService, DataService dataService)
        {
            this.excelPackage = excelPackage;
            this.appConfigService = appConfigService;
            this.dateService = dateService;
            this.dataService = dataService;
        }

        public ExcelService Export()
        {
            var market = excelPackage.Workbook.Worksheets["Event Market Offered"];
            var total = excelPackage.Workbook.Worksheets["Total"];
            excelPackage.Workbook.Worksheets.Delete(total);

            var dataTable = dataService.Query();

            var beginRowIndex = 3;
            var lastRowIndex = dataTable.Rows.Count + beginRowIndex - 1;
            var lastColumnIndex = dataTable.Columns.Count;

            RenderContent(market, total, dataTable, beginRowIndex, lastRowIndex, lastColumnIndex);
            RenderTotal(market, total, dataTable, lastRowIndex);

            return this;
        }

        private static void RenderTotal(ExcelWorksheet market, ExcelWorksheet total, DataTable dataTable, int lastRowIndex)
        {
            total.Cells["A2:G2"].Copy(market.Cells[lastRowIndex + 1, 1]);
            for (var columnIndex = 1; columnIndex < dataTable.Columns.Count; columnIndex++)
            {
                market.Cells[lastRowIndex + 1, columnIndex + 1].Value = dataTable.AsEnumerable().Sum(s => Converter.To<decimal>(s[columnIndex]));
            }
        }

        private void RenderContent(ExcelWorksheet market, ExcelWorksheet total, DataTable dataTable, int beginRowIndex, int lastRowIndex, int lastColumnIndex)
        {
            market.Cells["A1"].Value = $"Event Market Offered from {dateService.DateStart:dd MMM, yyyy} to {dateService.DateEnd:dd MMM, yyyy}";
            market.Cells["A2"].LoadFromDataTable(dataTable, true);
            market.Cells[beginRowIndex, 1, lastRowIndex, lastColumnIndex].StyleID = market.Cells["B3"].StyleID;

            market.Cells[$"D{beginRowIndex}:D{lastRowIndex}"].StyleID = total.Cells["D3"].StyleID;
            market.Cells[$"G{beginRowIndex}:G{lastRowIndex}"].StyleID = total.Cells["G3"].StyleID;
        }

        public string Save()
        {
            var filename = $"Event_Market_Offered_From_{dateService.DateStart:dd_MMM_yyyy}_To_{dateService.DateEnd:dd_MMM_yyyy}.xlsx";
            var folder = $@"{appConfigService.Configurations.ExportDataDirectory}\{dateService.DateStart:yyyy}";
            var fullName = $@"{folder}\{filename}";
            Directory.CreateDirectory(folder);
            excelPackage.SaveAs(new FileInfo(fullName), appConfigService.Configurations.ExcelPasswordProtected);

            return fullName;
        }
    }
}