using Fanex.SabaSoccerLetstalk.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch);
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

        public string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch)
        {
            UpdateRowIndex(data, batchIndex);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/SabaSoccerLetstalk.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            template.Cells["A3"].LoadFromCollection(data, false);

            RenderTitle(batchIndex, totalBatch, template);
            RenderData(data, template);

            return Save(batchIndex);
        }

        private static void RenderTitle(int batchIndex, int totalBatch, ExcelWorksheet template)
        {
            template.Cells["A1"].Value = $"Top Saba Soccer Punters - Page {batchIndex + 1} of {totalBatch}";
            var rptDate = AppConfigService.Config.RptDate;
            var date1 = rptDate.AddDays(-2);
            var date2 = rptDate;
            template.Cells["E1"].Value = $"{date1:yyyyMMdd} - {date2:yyyyMMdd}";
            template.Cells["H1"].Value = $"{date1:yyyyMM01} - {date2:yyyyMMdd}";
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

        private string Save(int batchIndex)
        {
            var date = dateRangeService.RptDate;
            var fileName = $"SabaSoccerLetstalk_Batch_{batchIndex + 1}_{date:yyyyMMdd}.xlsx";
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