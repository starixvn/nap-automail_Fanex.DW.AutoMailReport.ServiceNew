using Fanex.DHWL.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface ExcelMargin
    {
        string Export(List<MarginModel> data, bool isDaily);
    }

    public class ExcelMarginImp : ExcelMargin
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;

        private const int StartRowIndex = 3;

        public ExcelMarginImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
        }

        public string Export(List<MarginModel> data, bool isDaily)
        {
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
            var skipInsertBreakRow = new List<string> { "Others", "Total" };
            data.GroupBy(g => new { ProductGroup = g.ProductGroup }).ToList()
                .ForEach(productGroup =>
                {
                    if (endIndexGroup == starIndexGroup)
                    {
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

            template.Cells[endIndexGroup, 2, endIndexGroup, 15].StyleID = formatTpl.Cells["B4"].StyleID;
            formatTpl.Cells["B5:B7"].Copy(template.Cells[endIndexGroup + 1, 2]);
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

            template.Cells["B1"].Value = isDaily ? $"{todate:dd-MMM}" : "MTD";
            template.Cells["O1"].Value = isDaily ? $"{todate:dd-MMM}" : "MTD";
        }
    }
}