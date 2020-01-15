using Fanex.TopLicensee.Service.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Fanex.TopLicensee.Service.Services
{
    public interface ExcelService
    {
        string Export(Dictionary<string, List<LicenseeTOModel>> sports);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;

        public ExcelServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public string Export(Dictionary<string, List<LicenseeTOModel>> sports)
        {
            excelPackage = new ExcelPackage(new FileInfo("Templates/TopLicensee.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Template"];
            foreach (var sport in sports)
            {
                ExportBySport(template, sport);
            }

            return SaveCreatedFile();
        }

        private void ExportBySport(ExcelWorksheet template, KeyValuePair<string, List<LicenseeTOModel>> sport)
        {
            var sheet = excelPackage.Workbook.Worksheets.Add(sport.Key);
            template.Cells["B1:G4"].Copy(sheet.Cells["B1:G4"]);

            var data = RefineData(sport.Value);
            sheet.Cells["B2"].Value = sport.Key;
            sheet.Cells["D2"].Value = $"{dateService.GetRptDate():yyyyMMdd}";
            sheet.Cells["B4"].LoadFromCollection(data, false);
            var totalCol = 6;
            var startColIndex = 2;
            var startRowIndex = 4;
            var maxRowIndex = startRowIndex + sport.Value.Count - 1;
            for (int columnIndex = 0; columnIndex < totalCol; columnIndex++)
            {
                sheet.Cells[startRowIndex, startColIndex + columnIndex, maxRowIndex - 1, startColIndex + columnIndex].StyleID = sheet.Cells[startRowIndex, startColIndex + columnIndex].StyleID;
                sheet.Cells[maxRowIndex, startColIndex + columnIndex].StyleID = template.Cells[5, startColIndex + columnIndex].StyleID;
                sheet.Column(columnIndex + startColIndex).Width = template.Column(columnIndex + startColIndex).Width;
            }

            for (var rowIndex = startRowIndex - 2; rowIndex <= maxRowIndex; rowIndex++)
            {
                sheet.Row(rowIndex).Height = 19.5d;
            }
        }

        private List<LicenseeTOModel> RefineData(List<LicenseeTOModel> data)
        {
            int count = 1;
            data.ForEach(d =>
            {
                if (count <= 51)
                {
                    d.Order = count;
                }

                count++;
            });

            return data;
        }

        private string SaveCreatedFile()
        {
            var rptDate = dateService.GetRptDate();
            var fileName = $"Daily_Created_IBO_Customers_Report_For_{rptDate:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{rptDate:yyyy}\{rptDate:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.Workbook.Worksheets.Delete("Template");
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            return path;
        }
    }
}