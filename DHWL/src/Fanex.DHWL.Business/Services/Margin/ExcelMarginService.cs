using Fanex.DHWL.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface ExcelMarginService
    {
        string Export(DataTable dailyRawData, bool isDaily);

        int LastRowIndex { get; set; }
    }

    public class ExcelMarginServiceImp : ExcelMarginService
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;
        private readonly DataMarginService dataMarginService;

        private const int StartRowIndex = 3;

        public int LastRowIndex
        {
            get; set;
        }

        public ExcelMarginServiceImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth,
            DataMarginService dataMarginService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
            this.dataMarginService = dataMarginService;
        }

        public string Export(DataTable dailyRawData, bool isDaily)
        {
            var data = isDaily
                ? dataMarginService.CalcDataForDaily(dailyRawData)
                : dataMarginService.CalcDataForMTD(dailyRawData);
            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-Margin.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["Margin"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(dailyTemplate, isDaily);
            RenderData(data, dailyTemplate);
            FormatData(data, dailyTemplate, formatTpl);

            return Save(isDaily);
        }

        private void RenderData(List<MarginModel> data, ExcelWorksheet template)
        {
            template.Cells["B3"].LoadFromCollection(data, false);

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

        private void FormatData(List<MarginModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl)
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
                        template.Cells[endIndexGroup, 2, endIndexGroup, 15].StyleID = formatTpl.Cells["B2"].StyleID;
                        template.Row(endIndexGroup).Height = 5;
                    }
                });

            template.Cells[endIndexGroup, 2, endIndexGroup, 3].StyleID = formatTpl.Cells["B4"].StyleID;
            template.Cells[endIndexGroup, 3, endIndexGroup, 14].StyleID = formatTpl.Cells["C4"].StyleID;
            template.Cells[endIndexGroup, 15, endIndexGroup, 15].StyleID = formatTpl.Cells["B4"].StyleID;
            formatTpl.Cells["B5:B7"].Copy(template.Cells[endIndexGroup + 1, 2]);
            LastRowIndex = endIndexGroup;
        }

        private string Save(bool isDaily)
        {
            var date = dateService.DateTime.ToDate;
            var fileName = isDaily
                ? $"DHWL-Margin-Daily-{date:yyyy_MM_dd}.xlsx"
                : $"DHWL-Margin-MTD-{date:yyyy_MM_dd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        private void RenderTitle(ExcelWorksheet template, bool isDaily)
        {
            var now = DateTime.Now;
            var fromdate = dateService.DateTime.FromDate;
            var todate = dateService.DateTime.ToDate;
            var isEndOfMonth = todate.AddDays(1).Day == 1;
            var title = "MTD";
            if (isDaily)
            {
                title = $"{todate:dd-MMM}";
            }
            else if (isEndOfMonth)
            {
                title = $"{todate:MMM-yyyy}";
            }

            template.Cells["B1"].Value = title;
            template.Cells["O1"].Value = title;
        }
    }
}