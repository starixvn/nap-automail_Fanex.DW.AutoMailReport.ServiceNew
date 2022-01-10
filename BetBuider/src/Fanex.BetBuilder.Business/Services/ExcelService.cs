using System.Collections.Generic;
using System.IO;
using Fanex.BetBuilder.Business.Models;
using OfficeOpenXml;

namespace Fanex.BetBuilder.Business.Services
{
    public interface ExcelService
    {
        string Execute(List<MonthlyDataModel> data);
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly DateTimeService dateTimeService;
        private readonly ExcelPackage excelPackage;
        private readonly ExcelWorksheet monthlySheet;
        private readonly Log log;

        public ExcelServiceImp(ExcelPackage excelPackage, DateTimeService dateTimeService, Log log)
        {
            this.excelPackage = excelPackage;
            this.dateTimeService = dateTimeService;
            this.log = log;

            var sheetName = $"{dateTimeService.FromDate():MM-yyyy}";
            monthlySheet = excelPackage.Workbook.Worksheets["Template"];
            monthlySheet.Name = sheetName;
        }

        public string Execute(List<MonthlyDataModel> data)
        {
            RenderTitle();
            RenderData(data);
            var path = SaveFile();

            return path;
        }

        private void RenderTitle()
        {
            var title = $"{dateTimeService.FromDate():MM-yyyy} - GMT - 4";
            monthlySheet.Cells["B2"].Value = title;
        }

        private void RenderData(List<MonthlyDataModel> data)
        {
            const int RowIndex = 4;
            const int ColumnIndex = 1;
            const int MinWidth = 12;
            const int MaxWIdth = 200;
            var rowsCount = data.Count;

            monthlySheet.Cells[RowIndex, ColumnIndex].LoadFromCollection(data, false);

            var lastColumn = monthlySheet.Dimension.End.Column;
            for (var column = 1; column <= lastColumn; column++)
            {
                monthlySheet.Column(column).AutoFit(MinWidth, MaxWIdth);
                monthlySheet.Cells[RowIndex, column, rowsCount + RowIndex - 1, column].StyleID = monthlySheet.Cells[RowIndex, column].StyleID;
            }

            for (var row = RowIndex; row < rowsCount + RowIndex; row++)
            {
                monthlySheet.Row(row).Height = monthlySheet.Row(RowIndex).Height;
            }

            for (var column = 9; column <= lastColumn; column++)
            {
                for (var row = RowIndex; row < rowsCount + RowIndex; row++)
                {
                    if (monthlySheet.Cells[row, column].Value == null)
                    {
                        monthlySheet.Cells[row, column].Value = "-";
                        monthlySheet.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                }
            }
        }

        private string SaveFile()
        {
            var fromDate = dateTimeService.FromDate();
            var toDate = dateTimeService.ToDate();
            var filename = $"Bet_Builder_Monthly_Cross_Check_Report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx";

            var path = $@"{AppConfigService.AppConfig.FilePath}\{fromDate:yyyy}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{filename}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            log.Log(path);

            return path;
        }
    }
}