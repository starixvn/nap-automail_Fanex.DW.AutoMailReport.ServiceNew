using OfficeOpenXml;
using System.Data;
using System.IO;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(DataSet dataSet);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateRangeServiceProfier dateRangeServiceProfier;

        public ExcelServiceImp(ExcelPackage excelPackage, DateRangeServiceProfier dateRangeServiceProfier)
        {
            this.excelPackage = excelPackage;
            this.dateRangeServiceProfier = dateRangeServiceProfier;
        }

        public string ExportExcel(DataSet dataSet)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/{AppConfigService.Config.TemplateExcelFile}"));
            var index = 0;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                var template = excelPackage.Workbook.Worksheets[AppConfigService.Config.SheetExcelTemplateName[index]];
                template.Cells[AppConfigService.Config.CellAddressRender[index]].LoadFromDataTable(dataTable, false);

                var startRowIndex = AppConfigService.Config.StartExelRowIndex;
                var maxRowIndex = dataTable.Rows.Count + startRowIndex - 1;

                for (int columnIndex = 1; columnIndex <= dataTable.Columns.Count; columnIndex++)
                {
                    template.Cells
                        [
                        startRowIndex,
                        columnIndex,
                        maxRowIndex,
                        columnIndex
                        ].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;

                    template.Column(columnIndex).AutoFit(17, 50);
                }

                for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
                {
                    template.Row(rowIndex).Height = 18;
                }

                index++;
            }

            return Save();
        }

        private string Save()
        {
            var name = new FileInfo(AppConfigService.Config.TemplateExcelFile).Name.Replace(".xlsx", string.Empty);
            var date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange).ToDate;
            var fileName = $"{name}_For_{date:yyyy_MM_dd}.xlsx";
            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }
    }
}