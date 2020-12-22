using Fanex.DHWL.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk
{
    public interface MonthlyExcelDHWLService
    {
        string Export(DataTable monthlyRawData);

        int LastRowIndex { get; set; }
    }

    public class MonthlyExcelDHWLServiceImp : MonthlyExcelDHWLService
    {
        private readonly MonthlyDataDHWLService dataDHWLService;
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private const int StartRowIndex = 2;

        public int LastRowIndex
        {
            get; set;
        }

        public MonthlyExcelDHWLServiceImp(
            MonthlyDataDHWLService dataDHWLService,
            DateTimeServiceProvider dateService)
        {
            this.dataDHWLService = dataDHWLService;
            this.dateService = dateService;
        }

        public string Export(DataTable monthlyRawData)
        {
            var dataDHWL = dataDHWLService.CalTotalByProduct(monthlyRawData);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-Letstalk.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["Letstalk"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(dailyTemplate);
            RenderData(dataDHWL, dailyTemplate);
            FormatData(dataDHWL, dailyTemplate, formatTpl);

            return Save();
        }

        private void RenderData(List<DHWLModel> data, ExcelWorksheet template)
        {
            template.Cells["B2"].LoadFromCollection(data, false);

            var maxRowIndex = data.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = 2; columnIndex <= 7; columnIndex++)
            {
                if (columnIndex == 5)
                {
                    continue;
                }

                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        private void FormatData(List<DHWLModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl)
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
                        template.Cells[endIndexGroup, 2, endIndexGroup, 4].StyleID = formatTpl.Cells["B2"].StyleID;
                        template.Row(endIndexGroup).Height = 5;
                    }
                });

            endIndexGroup = endIndexGroup - 4;
            LastRowIndex = endIndexGroup + 5;
            for (var columnIndex = 2; columnIndex <= 4; columnIndex++)
            {
                var rowTplIndex = 4;
                for (var rowInex = endIndexGroup; rowInex <= endIndexGroup + 5; rowInex++)
                {
                    template.Cells[rowInex, columnIndex].StyleID = formatTpl.Cells[rowTplIndex, columnIndex].StyleID;
                    rowTplIndex++;
                }
            }
        }

        private string Save()
        {
            var date = dateService.DateTime.ToDate;
            var fileName = $"DHWL-Monthly-{date:yyyy_MM_dd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        private void RenderTitle(ExcelWorksheet template)
        {
            var todate = dateService.DateTime.ToDate;
            template.Cells["C1"].Value = $"{todate:dd-MMM-yyyy}";
        }
    }
}