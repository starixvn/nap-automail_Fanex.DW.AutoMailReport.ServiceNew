using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using BettypeWith3rdProducts.Business.Utility;
using Fanex.Common;
using OfficeOpenXml;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface ExcelService
    {
        string Execute();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ApplicationConfigurationService appConfig;
        private readonly DateRangeServiceProvider dateRangeServiceProvider;
        private readonly ExcelHeaderService excelHeaderService;
        private readonly DataService dataService;

        private readonly ExcelWorksheet dataTemplate;

        public ExcelServiceImp
            (ExcelPackage excelPackage,
            ApplicationConfigurationService appConfig,
            DataService dataService,
            DateRangeServiceProvider dateRangeServiceProvider,
            ExcelHeaderService excelHeaderService)
        {
            this.excelPackage = excelPackage;
            this.appConfig = appConfig;
            this.dataService = dataService;
            this.dateRangeServiceProvider = dateRangeServiceProvider;
            this.excelHeaderService = excelHeaderService;

            dataTemplate = excelPackage.Workbook.Worksheets["Data"];
        }

        public string Execute()
        {
            var dataSource = dataService.Execute();
            foreach (var dataOnSheet in dataSource)
            {
                var sheetName = dataOnSheet.Key;
                var excelWorksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                var sports = dataOnSheet.Value;
                ExportTitle(excelWorksheet, sheetName);
                FreezePanes(excelWorksheet);
                ExportData(excelWorksheet, sports);
            }

            ClearnUpTemplate();
            return Save();
        }

        private void ExportData(ExcelWorksheet excelWorksheet, Dictionary<string, List<DataTable>> sports)
        {
            foreach (var sport in sports)
            {
                foreach (var section in sport.Value)
                {
                    var startRowIndex = excelWorksheet.Dimension.End.Row;
                    var sportNameAddress = $"B{startRowIndex + 2}";
                    var sportHeaderStartRowAddress = $"B{startRowIndex + 5}";
                    excelWorksheet.Cells[sportNameAddress].Value = sport.Key;
                    excelWorksheet.Cells[sportNameAddress].StyleID = dataTemplate.Cells["B5"].StyleID;
                    excelWorksheet.Cells[sportHeaderStartRowAddress].LoadFromDataTable(section, false);
                    FotmatHeaderSportSection(excelWorksheet, section, startRowIndex);
                    FormatDateColumn(excelWorksheet, startRowIndex);
                    FormatDataSection(excelWorksheet, section, startRowIndex);
                    FormatTotal(excelWorksheet, section, startRowIndex);
                }
            }

            excelWorksheet.View.ShowGridLines = false;
        }

        private void FormatTotal(ExcelWorksheet excelWorksheet, DataTable section, int startRowIndex)
        {
            var startRow = startRowIndex + section.Rows.Count + 5;
            var startColumn = 2;
            excelWorksheet.InsertRow(startRow, 1);
            dataTemplate.Cells["B8"].Copy(excelWorksheet.Cells[startRow, startColumn]);
            startColumn++;

            for (var columnIndex = 1; columnIndex < section.Columns.Count; columnIndex++)
            {
                excelWorksheet.Cells[startRow, startColumn].Value = section.AsEnumerable().Sum(row => Converter.To<double>(row[columnIndex]));
                excelWorksheet.Cells[startRow, startColumn].StyleID = dataTemplate.Cells["C8"].StyleID;
                startColumn++;
            }
        }

        private void FormatDataSection(ExcelWorksheet excelWorksheet, DataTable section, int startRowIndex)
        {
            var startRow = startRowIndex + 5;
            var endRow = startRowIndex + section.Rows.Count + 4;
            var endColumn = section.Columns.Count + 1;
            excelWorksheet.Cells[startRow, 3, endRow, endColumn].StyleID = dataTemplate.Cells["C7"].StyleID;
            for (var columnIndex = 3; columnIndex < endColumn; columnIndex++)
            {
                for (var rowIndex = startRow; rowIndex < endRow; rowIndex++)
                {
                    var value = excelWorksheet.Cells[rowIndex, columnIndex].Value;
                    if (value != null)
                    {
                        excelWorksheet.Cells[rowIndex, columnIndex].Value = Converter.To<double>(value);
                    }
                }
            }
        }

        private void FormatDateColumn(ExcelWorksheet excelWorksheet, int startRowIndex)
        {
            var start = $"B{startRowIndex + 4}";
            var end = $"B{excelWorksheet.Dimension.End.Row}";
            excelWorksheet.Cells[$"{start}:{end}"].StyleID = dataTemplate.Cells["B7"].StyleID;
        }

        private void FotmatHeaderSportSection(ExcelWorksheet excelWorksheet, DataTable section, int startRowIndex)
        {
            var startRow = startRowIndex + 3;
            var endColumn = section.Columns.Count + 1;
            excelWorksheet.Cells[startRow, 2, startRow + 1, endColumn].StyleID = dataTemplate.Cells["B6"].StyleID;
            excelHeaderService.SplitHeader(excelWorksheet, section, startRow);
        }

        private void ExportTitle(ExcelWorksheet excelWorksheet, string sheetName)
        {
            var date = dateRangeServiceProvider.Date(appConfig.ReportType);
            excelPackage.Workbook.Worksheets[$"{sheetName} Temp"].Cells["B2:B5"].Copy(excelWorksheet.Cells["B2"]);
            excelWorksheet.Cells["B2"].Value = excelWorksheet.Cells["B2"].Value.ToString()
                .Replace("{0}", $"{date.DateStart:MM/dd/yyyy}")
                .Replace("{1}", $"{date.DateEnd:MM/dd/yyyy}");
            excelWorksheet.Cells["B3"].Value = excelWorksheet.Cells["B3"].Value.ToString()
                .Replace("{0}", $"{SystemTime.Now}");
            excelPackage.Workbook.Worksheets.Delete($"{sheetName} Temp");
        }

        private static void FreezePanes(ExcelWorksheet excelWorksheet)
        {
            var rowIndex = excelWorksheet.Dimension.End.Row + 1;
            excelWorksheet.View.FreezePanes(rowIndex, 3);
        }

        private string Save()
        {
            var folder = $@"{SystemTime.Now:yyyy}\{SystemTime.Now:MM}";
            var root = $@"{appConfig.ExportDataDirectory}\{folder}";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var fileName = $@"{root}\Bettype_With_3rd_Products_Monthly_Winloss_For_{SystemTime.Now:yyyy_MM_dd}.xlsx";
            excelPackage.SaveAs(new FileInfo(fileName));

            return fileName;
        }

        private void ClearnUpTemplate()
        {
            excelPackage.Workbook.Worksheets.Delete(dataTemplate);
        }
    }
}