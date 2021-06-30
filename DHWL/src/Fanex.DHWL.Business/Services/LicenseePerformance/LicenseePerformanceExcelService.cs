using Fanex.DHWL.Business.Models.LicenseeDHWLLetstalk;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services.LicenseePerformance
{
    public interface LicenseePerformanceExcelService
    {
        string Export(DataTable licPerfRawData, string imageName);

        int LastRowIndex { get; set; }
    }

    public class LicenseePerformanceExcelServiceImp : LicenseePerformanceExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;

        private readonly LicenseePerformanceDataService dataService;

        private const int StartRowIndex = 5;

        public int LastRowIndex
        {
            get; set;
        }

        public LicenseePerformanceExcelServiceImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService,
            LicenseePerformanceDataService dataService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
            this.dataService = dataService;
        }

        public string Export(DataTable licPerfRawData, string imageName)
        {
            var data = dataService.CalTotalByProduct(licPerfRawData);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/LicenseePerformance-Letstalk.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["Margin"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(dailyTemplate, imageName);
            RenderData(data, dailyTemplate);
            FormatData(data, dailyTemplate, formatTpl);

            return Save(imageName);
        }

        private void RenderData(List<LicenseePerformanceModel> data, ExcelWorksheet template)
        {
            template.Cells["B5"].LoadFromCollection(data, false);

            var maxRowIndex = data.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = 2; columnIndex <= 26; columnIndex++)
            {
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        private void FormatData(List<LicenseePerformanceModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var starIndexGroup = StartRowIndex;
            var endIndexGroup = starIndexGroup;
            var isStart = false;
            var skipInsertBreakRow = new List<string> { "Total", "Royalty Fees To 3rd parties", "NET TOTAL" };
            var productGroups = data.GroupBy(g => new { ProductGroup = g.ProductGroup }).Select(p => p.Key.ProductGroup).ToList();
            if (productGroups.Count > 1)
            {
                var beforeLastestGroup = productGroups[productGroups.Count - 2];
                skipInsertBreakRow.Add(beforeLastestGroup);
            }

            data.GroupBy(g => new { ProductGroup = g.ProductGroup }).ToList()
                .ForEach(productGroup =>
                {
                    if (endIndexGroup == starIndexGroup && !isStart)
                    {
                        isStart = true;
                        starIndexGroup = StartRowIndex;
                    }
                    else
                    {
                        starIndexGroup = endIndexGroup + 1;
                    }

                    endIndexGroup = starIndexGroup + productGroup.Count() - 1;

                    if (!skipInsertBreakRow.Contains(productGroup.Key.ProductGroup))
                    {
                        template.InsertRow(endIndexGroup + 1, 1);
                        starIndexGroup++;
                        endIndexGroup++;
                        template.Cells[endIndexGroup, 2, endIndexGroup, 9].StyleID = formatTpl.Cells["B2"].StyleID;
                        template.Row(endIndexGroup).Height = 5;
                    }
                });

            int tplRowIndex = 4;
            var columnsClearText = new List<int> { 3, 4, 5, 6, 7, 8 };
            for (int rowIndex = endIndexGroup - 2; rowIndex <= endIndexGroup; rowIndex++)
            {
                for (int columnIndex = 2; columnIndex <= 9; columnIndex++)
                {
                    template.Cells[rowIndex, columnIndex].StyleID = formatTpl.Cells[tplRowIndex, columnIndex].StyleID;
                    if (columnsClearText.Contains(columnIndex) && tplRowIndex == 5)
                    {
                        template.Cells[rowIndex, 2, rowIndex, 8].Merge = true;
                    }
                }
                if (tplRowIndex == 5)
                {
                    template.Cells[rowIndex, 2].Style.WrapText = true;
                    template.Cells[rowIndex, 15].Style.WrapText = true;
                }

                tplRowIndex++;
            }

            formatTpl.Cells["B7:B8"].Copy(template.Cells[endIndexGroup + 1, 2]);
            LastRowIndex = endIndexGroup;
        }

        private string Save(string imageName)
        {
            var date = dateService.DateTime.ToDate;
            var fileName = $"{imageName}-{date:yyyy_MM_dd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        private void RenderTitle(ExcelWorksheet template, string imageName)
        {
            var todate = dateService.DateTime.ToDate;
            var isEndOfMonth = todate.AddDays(1).Day == 1;
            var title = $"{todate:dd-MMM-yy}";
            var header = imageName.Equals("SB-LicPef")
                ? "Sportsbook Licensee Performance (RMB)"
                : "3rd Product Licensee Performance (RMB)";

            template.Cells["C3"].Value = title;
            template.Cells["B2"].Value = header;

            title = $"MTD ({todate:dd-MMM})";
            if (isEndOfMonth)
            {
                title = $"{todate:MMM-yyyy}";
            }

            template.Cells["F3"].Value = title;
        }
    }
}