using Fanex.Dangerous789YCustomer.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.Dangerous789YCustomer.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, Dangerous789YCustomerettings setting);

        string ExportExcelWithNoData(Dangerous789YCustomerettings setting);
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

        public string ExportExcel(List<PunterModel> data, int batchIndex, int totalBatch, bool isAllInfo, Dangerous789YCustomerettings setting)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/Dangerous789YCustomer.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            template.Cells["B5"].LoadFromCollection(data, false);

            RenderTitle(batchIndex, totalBatch, template, isAllInfo, setting);
            RenderData(data, template);

            return Save(batchIndex, isAllInfo, setting);
        }

        private void RenderTitle(int batchIndex, int totalBatch, ExcelWorksheet template, bool isAllInfo, Dangerous789YCustomerettings setting)
        {
            var title = isAllInfo
                ? setting.Exceltitle
                : $"{setting.Exceltitle} - Page {batchIndex + 1} of {totalBatch}";

            template.Cells["B2"].Value = title;
            template.Cells["D3"].Value = dateTitleService.DateGroup1();
            template.Cells["G3"].Value = dateTitleService.DateGroup2();
            template.Cells["J3"].Value = dateTitleService.DateGroup3();
        }

        private void RenderData(List<PunterModel> data, ExcelWorksheet template)
        {
            int i = 0;
            string rootName = string.Empty;
            foreach (var row in data)
            {
                if (i == 0 || i == 5)
                {
                    rootName = row.UserName;
                    i = 0;
                }
                if (i > 0)
                {
                    row.AssociatedAccount = rootName;
                    row.IsRootMember = false;
                }

                i++;
            }

            var startRowIndex = 5;
            var maxRowIndex = data.Count + startRowIndex - 1;
            var templateNote = excelPackage.Workbook.Worksheets["Note"];

            var username = string.Empty;
            var associatedAccount = string.Empty;
            var rowCountGroup = 0;
            var rowOddEven = 0;

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                var row = data[rowIndex - startRowIndex];
                if (row.IsRootMember)
                {
                    rowOddEven = rowCountGroup % 2 == 0 ? 6 : 7;
                    rowCountGroup++;
                }
                else
                {
                    rowOddEven = rowCountGroup % 2 == 0 ? 9 : 8;
                }

                for (int columnIndex = 1; columnIndex <= 13; columnIndex++)
                {
                    template.Cells[rowIndex, columnIndex].StyleID = templateNote.Cells[rowOddEven, columnIndex].StyleID;
                }

                template.Row(rowIndex).Height = 19.5;
            }

            templateNote.Cells["B2:B3"].Copy(template.Cells[maxRowIndex + 1, 2]);
        }

        private string Save(int batchIndex, bool isAllInfo, Dangerous789YCustomerettings setting)
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
            excelPackage.Workbook.Worksheets.Delete("Note");
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        public string ExportExcelWithNoData(Dangerous789YCustomerettings setting)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/Dangerous789YCustomer-NoData.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Data"];
            RenderTitleWithNoData(setting.Exceltitle, template);

            return Save(0, true, setting);
        }

        private void RenderTitleWithNoData(string exceltitle, ExcelWorksheet template)
        {
            template.Cells["B2"].Value = exceltitle;
            template.Cells["D3"].Value = dateTitleService.DateGroup1();
            template.Cells["G3"].Value = dateTitleService.DateGroup2();
            template.Cells["J3"].Value = dateTitleService.DateGroup3();
        }
    }
}