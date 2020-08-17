using Fanex.TopSportbookAgents.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Fanex.TopSportbookAgents.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, TopSportbookAgentSettings setting);

        string ExportExcelWithNoData(TopSportbookAgentSettings setting);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateRangeService dateRangeService;
        private readonly DateTitleService dateTitleService;

        public ExcelServiceImp(ExcelPackage excelPackage, DateRangeService dateRangeService, DateTitleService dateTitleService)
        {
            this.excelPackage = excelPackage;
            this.dateRangeService = dateRangeService;
            this.dateTitleService = dateTitleService;
        }

        public string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, TopSportbookAgentSettings setting)
        {
            UpdateRowIndex(data, batchIndex);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/TopSportbookAgents.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            template.Cells["A3"].LoadFromCollection(data, false);

            RenderTitle(batchIndex, totalBatch, template, isAllInfo, setting);
            RenderData(data, template);

            return Save(batchIndex, isAllInfo, setting);
        }

        private void RenderTitle(int batchIndex, int totalBatch, ExcelWorksheet template, bool isAllInfo, TopSportbookAgentSettings setting)
        {
            var title = isAllInfo
                ? setting.Exceltitle
                : $"{setting.Exceltitle} - Page {batchIndex + 1} of {totalBatch}";

            template.Cells["A1"].Value = title;
            template.Cells["E1"].Value = dateTitleService.DateGroup1();
            template.Cells["K1"].Value = dateTitleService.DateGroup3();
        }

        private static void RenderData(List<PunterModel> data, ExcelWorksheet template)
        {
            var startRowIndex = 3;
            var maxRowIndex = data.Count + startRowIndex - 1;

            for (int columnIndex = 1; columnIndex <= 14; columnIndex++)
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

        private string Save(int batchIndex, bool isAllInfo, TopSportbookAgentSettings setting)
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

        public string ExportExcelWithNoData(TopSportbookAgentSettings setting)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/TopSportbookAgents-NoData.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            RenderTitleWithNoData(setting.Exceltitle, template);

            return Save(0, true, setting);
        }

        private void RenderTitleWithNoData(string exceltitle, ExcelWorksheet template)
        {
            template.Cells["A1"].Value = $"{exceltitle}";
            template.Cells["E1"].Value = dateTitleService.DateGroup1();
            template.Cells["K1"].Value = dateTitleService.DateGroup3();
        }
    }
}