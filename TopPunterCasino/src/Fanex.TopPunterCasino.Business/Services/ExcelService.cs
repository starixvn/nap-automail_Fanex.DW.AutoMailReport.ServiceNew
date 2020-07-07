using Fanex.TopPunterCasino.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.TopPunterCasino.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, TopPunterCasinoSetting setting);

        string ExportExcelWithNoData(TopPunterCasinoSetting setting);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateRangeService dateRangeService;

        public ExcelServiceImp(ExcelPackage excelPackage, DateRangeService dateRangeService)
        {
            this.excelPackage = excelPackage;
            this.dateRangeService = dateRangeService;
        }

        public string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, TopPunterCasinoSetting setting)
        {
            UpdateRowIndex(data, batchIndex);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/TopPunterCasino.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            template.Cells["A3"].LoadFromCollection(data, false);
            template.Column(11).Hidden = true;

            RenderTitle(batchIndex, totalBatch, template, isAllInfo, setting);
            RenderData(data, template);

            return Save(batchIndex, isAllInfo, setting);
        }

        private void RenderTitle(int batchIndex, int totalBatch, ExcelWorksheet template, bool isAllInfo, TopPunterCasinoSetting setting)
        {
            var title = isAllInfo
                ? setting.Exceltitle
                : $"{setting.Exceltitle} - Page {batchIndex + 1} of {totalBatch}";

            template.Cells["A1"].Value = title;
            var rptDate = dateRangeService.RptDate;
            var date1 = rptDate.AddDays(-5);
            var date2 = rptDate;
            template.Cells["E1"].Value = $"{date1:yyyyMMdd} - {date2:yyyyMMdd}";
            template.Cells["H1"].Value = "MTD";
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

            template.Column(12).Hidden = true;
            template.Column(13).Hidden = true;
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

        private string Save(int batchIndex, bool isAllInfo, TopPunterCasinoSetting setting)
        {
            var date = dateRangeService.RptDate;
            var fileName = isAllInfo
                ? $"{setting.Exceltitle}_{date:yyyyMMdd}.xlsx"
                : $"{setting.Exceltitle}_Batch_{batchIndex + 1}_{date:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        public string ExportExcelWithNoData(TopPunterCasinoSetting setting)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/TopPunterCasino-NoData.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            var date = dateRangeService.RptDate;
            RenderTitleWithNoData(setting.Exceltitle, template, date);
            template.Column(11).Hidden = true;

            return Save(0, true, setting);
        }

        private static void RenderTitleWithNoData(string exceltitle, ExcelWorksheet template, DateTime date)
        {
            var date1 = date.AddDays(-5);
            var date2 = date;
            template.Cells["A1"].Value = $"{exceltitle}";
            template.Cells["E1"].Value = $"{date1:yyyyMMdd} - {date2:yyyyMMdd}";
            template.Cells["H1"].Value = "MTD";
        }
    }
}