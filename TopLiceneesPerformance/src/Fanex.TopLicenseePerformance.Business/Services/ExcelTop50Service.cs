using Fanex.TopLicensee.Service.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.TopLicensee.Service.Services
{
    public interface ExcelTop50Service
    {
        string Export(string batch, Dictionary<string, List<LicenseeTOModel>> sports);

        int LastRowIndex { get; set; }
    }

    public class ExcelTop50ServiceImp : ExcelTop50Service
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;

        public int LastRowIndex
        {
            get; set;
        }

        public ExcelTop50ServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public string Export(string batch, Dictionary<string, List<LicenseeTOModel>> sports)
        {
            excelPackage = new ExcelPackage(new FileInfo("Templates/TopLicensee.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Template"];
            foreach (var sport in sports)
            {
                ExportBySport(template, sport);
            }

            return SaveCreatedFile(batch);
        }

        private void ExportBySport(ExcelWorksheet template, KeyValuePair<string, List<LicenseeTOModel>> sport)
        {
            var data = sport.Value;
            var sheet = excelPackage.Workbook.Worksheets.Add(sport.Key);
            template.Cells["B1:M4"].Copy(sheet.Cells["B1:M4"]);
            sheet.View.FreezePanes(4, 5);

            sheet.Cells["B2"].Value = sport.Key;
            sheet.Cells["E2"].Value = $"{dateService.ToDate:yyyyMMdd}";
            sheet.Cells["B4"].LoadFromCollection(data, false);

            int startColIndex, startRowIndex, maxRowIndex;

            FormatGroup(template, sport, sheet, out startColIndex, out startRowIndex, out maxRowIndex);
            MergeGroup(sheet, data, startColIndex, startRowIndex, maxRowIndex);
        }

        private void FormatGroup(ExcelWorksheet template, KeyValuePair<string, List<LicenseeTOModel>> sport, ExcelWorksheet sheet, out int startColIndex, out int startRowIndex, out int maxRowIndex)
        {
            var totalCol = 12;
            startColIndex = 2;
            startRowIndex = 4;
            maxRowIndex = startRowIndex + sport.Value.Count - 1;
            LastRowIndex = maxRowIndex;
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

        private static void MergeGroup(ExcelWorksheet sheet, List<LicenseeTOModel> data, int startColIndex, int startRowIndex, int maxRowIndex)
        {
            var currentIndex = startRowIndex;
            var index = 1;
            data.GroupBy(g => new { g.LicGroup })
                .ToList()
                .ForEach(group =>
                {
                    var cnt = group.Count();

                    sheet.Cells[currentIndex, 2, currentIndex + cnt - 1, 2].Merge = true;
                    sheet.Cells[currentIndex, 2, currentIndex + cnt - 1, 2].Value = index;

                    sheet.Cells[currentIndex, 3, currentIndex + cnt - 1, 3].Merge = true;
                    sheet.Cells[currentIndex, 5, currentIndex + cnt - 1, 5].Merge = true;
                    sheet.Cells[currentIndex, 6, currentIndex + cnt - 1, 6].Merge = true;
                    sheet.Cells[currentIndex, 7, currentIndex + cnt - 1, 7].Merge = true;
                    sheet.Cells[currentIndex, 8, currentIndex + cnt - 1, 8].Merge = true;

                    currentIndex += cnt;
                    index++;
                });

            var value = sheet.Cells[maxRowIndex, startColIndex + 1].Value;
            sheet.Cells[maxRowIndex, startColIndex, maxRowIndex, startColIndex + 2].Merge = true;
            sheet.Cells[maxRowIndex, startColIndex, maxRowIndex, startColIndex + 2].Value = value;
            sheet.Cells[maxRowIndex, startColIndex, maxRowIndex, startColIndex + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        private string SaveCreatedFile(string batch)
        {
            var rptDate = dateService.ToDate;
            var fileName = $"Top_Licensees_by_TO_{batch}_For_{rptDate:yyyyMMdd}.xlsx";
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