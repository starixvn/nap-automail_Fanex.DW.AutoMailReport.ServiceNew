using OfficeOpenXml;
using System.Data;
using System.Drawing;
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

                for (int columnIndex = AppConfigService.Config.StartExcelColumnIndex; columnIndex <= dataTable.Columns.Count; columnIndex++)
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

                // format footer
                for (int columnIndex = AppConfigService.Config.StartExcelColumnIndex; columnIndex <= dataTable.Columns.Count; columnIndex++)
                {
                    template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].Style.Font.Bold = true;
                    template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(83, 131, 213));
                    template.Cells[maxRowIndex, 1, maxRowIndex, 2].Merge = true;
                    template.Cells[maxRowIndex, 1, maxRowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                index++;
            }

            return Save();
        }

        private string Save()
        {
            var date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange).ToDate;
            var fileName = $"{AppConfigService.Config.FileName}_{date:yyyy_MM_dd}.xlsx";
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