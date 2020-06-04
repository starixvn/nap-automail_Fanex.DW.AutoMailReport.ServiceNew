using Fanex.SabaSoccerLetstalk.Business.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface SabaSportPunterExcelService
    {
        string ExportExcelWithData(Sport sport, string exceltitle, DateTime date, List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo);

        string ExportExcelWithNoData(Sport sport, string exceltitle, DateTime date);
    }

    public class SabaSportPunterExcelServiceImp : SabaSportPunterExcelService
    {
        private ExcelPackage excelPackage;

        public SabaSportPunterExcelServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        public string ExportExcelWithData(Sport sport, string exceltitle, DateTime date, List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo)
        {
            UpdateRowIndex(data, batchIndex);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/SabaSoccerLetstalk.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            var startIndexRow = 3;
            template.Cells["A3"].LoadFromCollection(data, false);

            RenderTitleWithData(exceltitle, batchIndex, totalBatch, template, date);
            RenderData(data, template);
            RenderHighLightBGColor(data, template, startIndexRow);

            template.Column(12).Hidden = true;
            template.Column(13).Hidden = true;

            return isAllInfo ? Save(sport, date) : Save(sport, date, batchIndex);
        }

        private static void RenderHighLightBGColor(List<PunterModel> data, ExcelWorksheet template, int startIndexRow)
        {
            for (var rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                var row = data[rowIndex];
                if (row.IsSetBGColor)
                {
                    template.Cells[rowIndex + startIndexRow, 1, rowIndex + startIndexRow, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    template.Cells[rowIndex + startIndexRow, 1, rowIndex + startIndexRow, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(250, 191, 143));
                }
            }
        }

        private static void RenderTitleWithData(string exceltitle, int batchIndex, int totalBatch, ExcelWorksheet template, DateTime date)
        {
            template.Cells["A1"].Value = $"{exceltitle} - Page {batchIndex + 1} of {totalBatch}";
            template.Cells["E1"].Value = $"{date:yyyyMMdd} - Time Range: {AppConfigService.Config.TimeRange}";
        }

        private static void RenderTitleWithNoData(string exceltitle, ExcelWorksheet template, DateTime date)
        {
            template.Cells["A1"].Value = $"{exceltitle}";
            template.Cells["E1"].Value = $"{date:yyyyMMdd} - Time Range: {AppConfigService.Config.TimeRange}";
        }

        private static void RenderData(List<PunterModel> data, ExcelWorksheet template)
        {
            var startRowIndex = 3;
            var maxRowIndex = data.Count + startRowIndex - 1;

            for (int columnIndex = 1; columnIndex <= 10; columnIndex++)
            {
                template.Cells
                    [
                    startRowIndex,
                    columnIndex,
                    maxRowIndex,
                    columnIndex
                    ].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;
            }

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }
        }

        private static void UpdateRowIndex(List<PunterModel> data, int batchIndex)
        {
            var total = 50;
            var index = total * (batchIndex) + 1;
            data.ForEach(row =>
            {
                row.Index = index;
                index++;
            });
        }

        private string Save(Sport sport, DateTime date, int batchIndex)
        {
            var fileName = $"{sport}_Batch_{batchIndex + 1}_{date:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        private string Save(Sport sport, DateTime date)
        {
            var fileName = $"{sport}_Batch_{AppConfigService.Config.TimeRange}_{date:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        public string ExportExcelWithNoData(Sport sport, string exceltitle, DateTime date)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/SabaSoccerLetstalk-NoData.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];

            RenderTitleWithNoData(exceltitle, template, date);

            return Save(sport, date, 0);
        }
    }
}