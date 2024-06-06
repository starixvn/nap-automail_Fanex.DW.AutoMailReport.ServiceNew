using Fanex.DHWL.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface ExcelMarginHedgingSpecialService
    {
        string Export(DataTable dailyRawData, bool isDaily, int reportId);

        int LastRowIndex { get; set; }

        List<MarginHedgingSpecialModel> GrandTotalList { get; set; }
    }

    public class ExcelMarginHedgingSpecialServiceImp : ExcelMarginHedgingSpecialService
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly DataMarginHedgingSpecialService dataMarginService;

        private const int StartRowIndex = 4;

        public int LastRowIndex
        {
            get; set;
        }

        public ExcelMarginHedgingSpecialServiceImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService,
            DataMarginHedgingSpecialService dataMarginService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
            this.dataMarginService = dataMarginService;
        }

        public List<MarginHedgingSpecialModel> GrandTotalList { get; set; }

        public string Export(DataTable dailyRawData, bool isDaily, int reportId)
        {
            var data = isDaily
                 ? dataMarginService.CalcDataForDaily(dailyRawData)
                 : dataMarginService.CalcDataForMtd(dailyRawData);

            GrandTotalList = dataMarginService.GrandTotalList;
            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-MarginHedgingSpecial.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["Margin"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(dailyTemplate, isDaily, reportId);
            RenderData(data, dailyTemplate);
            FormatData(data, dailyTemplate, formatTpl);

            return Save(isDaily);
        }

        private void RenderData(List<MarginHedgingSpecialModel> data, ExcelWorksheet template)
        {
            template.Cells["B4"].LoadFromCollection(data, false);

            var maxRowIndex = data.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = 2; columnIndex <= 36; columnIndex++)
            {
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        private void FormatData(List<MarginHedgingSpecialModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var starIndexGroup = StartRowIndex;
            var endIndexGroup = starIndexGroup;
            var isStart = false;
            var skipInsertBreakRow = new List<string> { "Total" };
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
                        template.Cells[endIndexGroup, 2, endIndexGroup, 21].StyleID = formatTpl.Cells["B2"].StyleID;
                        template.Row(endIndexGroup).Height = 5;
                    }
                });

            template.Cells[endIndexGroup - 2, 2, endIndexGroup - 2, 3].StyleID = formatTpl.Cells["B4"].StyleID;
            template.Cells[endIndexGroup - 2, 3, endIndexGroup - 2, 14].StyleID = formatTpl.Cells["C4"].StyleID;
            template.Cells[endIndexGroup - 2, 21, endIndexGroup - 2, 21].StyleID = formatTpl.Cells["B4"].StyleID;

            int tplRowIndex = 4;
            var columnsClearText = new List<int> { 3, 5, 6, 8, 9, 11, 12, 14, 15, 17, 18, 20 };
            for (int rowIndex = endIndexGroup - 2; rowIndex <= endIndexGroup; rowIndex++)
            {
                for (int columnIndex = 2; columnIndex <= 21; columnIndex++)
                {
                    template.Cells[rowIndex, columnIndex].StyleID = formatTpl.Cells[tplRowIndex, columnIndex].StyleID;
                    if (columnsClearText.Contains(columnIndex) && tplRowIndex > 4)
                    {
                        template.Cells[rowIndex, columnIndex].Value = "";
                    }
                }
                if (tplRowIndex == 5)
                {
                    template.Row(rowIndex).Height = 27.6;
                    template.Cells[rowIndex, 2].Style.WrapText = true;
                    template.Cells[rowIndex, 21].Style.WrapText = true;
                }

                tplRowIndex++;
            }

            formatTpl.Cells["B7:B9"].Copy(template.Cells[endIndexGroup + 1, 2]);
            LastRowIndex = endIndexGroup + 4;
        }

        private string Save(bool isDaily)
        {
            var date = dateService.DateTime.ToDate;
            var fileName = isDaily
                ? $"DHWL-Margin-HedgingSpecial-Daily-{date:yyyy_MM_dd}.xlsx"
                : $"DHWL-Margin-HedgingSpecial-MTD-{date:yyyy_MM_dd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        private void RenderTitle(ExcelWorksheet template, bool isDaily, int reportId)
        {
            var todate = dateService.DateTime.ToDate;
            var isEndOfMonth = todate.AddDays(1).Day == 1;
            var title = $"MTD ({todate:dd-MMM})";
            if (isDaily)
            {
                title = $"{todate:dd-MMM}";
            }
            else if (isEndOfMonth)
            {
                title = $"{todate:MMM-yyyy}";
            }

            template.Cells["U1"].Value = $"{template.Cells["U1"].Value}.{reportId}";
            template.Cells["B2"].Value = title;
            template.Cells["U2"].Value = title;
        }
    }
}