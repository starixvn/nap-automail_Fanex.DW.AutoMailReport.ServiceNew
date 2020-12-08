using Fanex.HouseMarginMonitoring.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.HouseMarginMonitoring.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<BettypeModel> data, int batchIndex, int totalBatch, bool isAllInfo, HouseMarginSettings setting);

        string ExportExcelWithNoData(HouseMarginSettings setting);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;

        public ExcelServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        public string ExportExcel(List<BettypeModel> data, int batchIndex, int totalBatch, bool isAllInfo, HouseMarginSettings setting)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/HouseMarginMonitoring.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            var tplFormat = excelPackage.Workbook.Worksheets["Format"];
            template.Cells["B4"].LoadFromCollection(data, false);

            RenderTitle(template);
            RenderData(data, template, tplFormat);

            return Save(batchIndex, isAllInfo, setting);
        }

        private void RenderData(List<BettypeModel> data, ExcelWorksheet template, ExcelWorksheet tplFormat)
        {
            var startRowIndex = 4;
            var maxRowIndex = data.Count + startRowIndex - 1;

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                var row = data[rowIndex - startRowIndex];
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = 2; columnIndex <= 6; columnIndex++)
            {
                template.Cells[startRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;
                template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = tplFormat.Cells[4, columnIndex].StyleID;
            }

            template.Cells[maxRowIndex, 6].Value = string.Empty;
            template.Cells[maxRowIndex, 2, maxRowIndex, 3].Value = "Total";
            template.Cells[maxRowIndex, 2, maxRowIndex, 3].Merge = true;
        }

        private string Save(int batchIndex, bool isAllInfo, HouseMarginSettings setting)
        {
            var date = DateTime.Now;
            var fileName = isAllInfo
                ? $"{setting.Exceltitle} {date:yyyyMMdd HH00}.xlsx"
                : $"{setting.Exceltitle} Batch {batchIndex + 1} {date:yyyyMMdd HH00}.xlsx";
            var path = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}\{date:dd}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        public string ExportExcelWithNoData(HouseMarginSettings setting)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/HouseMarginMonitoring-NoData.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            RenderTitle(template);

            return Save(0, true, setting);
        }

        private void RenderTitle(ExcelWorksheet template)
        {
            var now = DateTime.Now;
            var date = AppConfigService.Config.IsManual
                ? AppConfigService.Config.ToDate
                : now;
            var title = $"House Margin Monitoring - {date:yyyyMMdd} - Time Range: 00:00 - {now:HH:00}";
            template.Cells["B2"].Value = title;
        }
    }
}