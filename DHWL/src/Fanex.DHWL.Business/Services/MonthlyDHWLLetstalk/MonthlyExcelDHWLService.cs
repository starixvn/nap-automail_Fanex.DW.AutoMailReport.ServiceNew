using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.Adjustment;
using Fanex.DHWL.Business.Services.MonthlyReconciliation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk
{
    public interface MonthlyExcelDHWLService
    {
        string Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBA);

        int LastRowIndex { get; set; }

        int LastAdjustmentRowIndex { get; set; }
    }

    public class MonthlyExcelDHWLServiceImp : MonthlyExcelDHWLService
    {
        private readonly MonthlyDataDHWLService dataDHWLService;
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly AdjustmentDataService adjustmentService;
        private readonly MonthlyReconciliationExcelService reconciliation;
        private readonly MonthlyBAExcelService monthlyBAExcelService;
        private readonly ExcelMarginService excelMarginService;

        private const int StartRowIndex = 4;

        public int LastRowIndex
        {
            get; set;
        }

        public int LastAdjustmentRowIndex
        {
            get; set;
        }

        public MonthlyExcelDHWLServiceImp(
            MonthlyDataDHWLService dataDHWLService,
            DateTimeServiceProvider dateService,
            AdjustmentDataService adjustmentService,
            MonthlyReconciliationExcelService reconciliation,
            MonthlyBAExcelService monthlyBAExcelService,
            ExcelMarginService excelMarginService)
        {
            this.dataDHWLService = dataDHWLService;
            this.dateService = dateService;
            this.adjustmentService = adjustmentService;
            this.monthlyBAExcelService = monthlyBAExcelService;
            this.reconciliation = reconciliation;
            this.excelMarginService = excelMarginService;
        }

        public string Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBA)
        {
            var dataDHWL = dataDHWLService.CalTotalByProduct(monthlyRawData);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-Letstalk.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Letstalk"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            var marginSheet = excelPackage.Workbook.Worksheets.Add("Monthly Margin");
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(template);
            RenderData(dataDHWL, template);
            FormatData(dataDHWL, template, formatTpl);
            RenderAdjustment(monthlyRawData, template);
            RenderReconciliation(dataReconciliation);
            RenderBA(dataBA);
            RenderMTDMargin(monthlyRawData, marginSheet);

            return Save();
        }

        private void RenderMTDMargin(DataTable monthlyRawData, ExcelWorksheet marginSheet)
        {
            var mtdMarginExcelPath = excelMarginService.Export(monthlyRawData, false);
            var excelPackageMargin = new ExcelPackage(new FileInfo(mtdMarginExcelPath), AppConfigService.Config.Password);
            var mtdMarginSheet = excelPackageMargin.Workbook.Worksheets["Margin"];

            mtdMarginSheet.Cells[1, 2, excelMarginService.LastRowIndex, 15].Copy(marginSheet.Cells["A5"]);
            marginSheet.View.ShowGridLines = false;
            marginSheet.Cells["A2"].Value = $"Monthly Margin Report For {dateService.DateTime.ToDate: MMM yyyy}";
            marginSheet.Cells["A2"].Style.Font.Bold = true;
            marginSheet.Cells["A2"].Style.Font.Size = 14;

            marginSheet.Cells["A3"].Value = $"Generated on {DateTime.Now:dd MMM yyyy hh:mm:ss tt}";

            for (int rowIndex = 1; rowIndex <= excelMarginService.LastRowIndex; rowIndex++)
            {
                marginSheet.Row(rowIndex + 4).Height = mtdMarginSheet.Row(rowIndex).Height;
            }

            for (int colIndex = 1; colIndex <= 15; colIndex++)
            {
                marginSheet.Column(colIndex).Width = mtdMarginSheet.Column(colIndex + 1).Width;
            }
        }

        private void RenderData(List<MonthlyDHWLModel> data, ExcelWorksheet template)
        {
            template.Cells["B4"].LoadFromCollection(data, false);

            var maxRowIndex = data.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = 2; columnIndex <= 7; columnIndex++)
            {
                if (columnIndex == 4)
                {
                    continue;
                }

                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        private void FormatData(List<MonthlyDHWLModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var starIndexGroup = StartRowIndex;
            var endIndexGroup = starIndexGroup;
            var isStart = false;
            var skipInsertBreakRow = new List<string> { "Total", "Adjustment", "Net Income" };
            var productGroups = data.GroupBy(g => new { ProductGroup = g.ProductGroup }).Select(p => p.Key.ProductGroup).ToList();
            if (productGroups.Count > 4)
            {
                var beforeLastestGroup = productGroups[productGroups.Count - 4];
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
                        template.Cells[endIndexGroup, 2, endIndexGroup, 3].StyleID = formatTpl.Cells["B2"].StyleID;
                        template.Row(endIndexGroup).Height = 5;
                    }
                });

            endIndexGroup = endIndexGroup - 2;
            LastRowIndex = endIndexGroup + 3;
            for (var columnIndex = 2; columnIndex <= 3; columnIndex++)
            {
                var rowTplIndex = 4;
                for (var rowInex = endIndexGroup; rowInex <= endIndexGroup + 5; rowInex++)
                {
                    template.Cells[rowInex, columnIndex].StyleID = formatTpl.Cells[rowTplIndex, columnIndex].StyleID;
                    rowTplIndex++;
                }
            }

            template.Column(5).Hidden = true;
            template.Column(6).Hidden = true;
            template.Column(7).Hidden = true;
        }

        private void RenderAdjustment(DataTable monthlyRawData, ExcelWorksheet template)
        {
            var todate = dateService.DateTime.ToDate;
            var dataAjustment = adjustmentService.CalcData(monthlyRawData);
            template.Cells["I2"].Value = $"Explanation for Adjustment - {todate:MMM yyyy}";
            template.Cells["I4"].LoadFromCollection(dataAjustment, false);

            LastAdjustmentRowIndex = dataAjustment.Count + 3;
        }

        private string Save()
        {
            var date = dateService.DateTime.ToDate;
            var fileName = $"Monthly_HWL_Report_and_Licensee_Rebate_&_Discount_Data_Soft_Copy_Files_For_{date:yyyy_MM}.xlsx";

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
            template.Cells["B2"].Value = $"Monthly House Net Income - {todate:MMM yyyy}";
        }

        private void RenderReconciliation(DataTable dataConciliation)
        {
            var tplReconciliation = excelPackage.Workbook.Worksheets["Rebate & Discount"];
            var date = dateService.DateTime.ToDate;
            reconciliation.RenderReconciliation(tplReconciliation, dataConciliation, date);
        }

        private void RenderBA(DataTable dataBA)
        {
            var tplBA = excelPackage.Workbook.Worksheets["BA"];
            var date = dateService.DateTime.ToDate;
            monthlyBAExcelService.RenderBA(tplBA, dataBA, date);
        }
    }
}