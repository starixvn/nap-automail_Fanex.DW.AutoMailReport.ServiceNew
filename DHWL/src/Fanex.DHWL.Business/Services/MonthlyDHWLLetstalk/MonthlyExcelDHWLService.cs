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
        string Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBA, int reportId);

        int LastRowIndex { get; set; }

        int LastAdjustmentRowIndex { get; set; }
    }

    public class MonthlyExcelDHWLServiceImp : MonthlyExcelDHWLService
    {
        private readonly MonthlyDataDHWLService dataDhwlService;
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly AdjustmentDataService adjustmentService;
        private readonly MonthlyReconciliationExcelService reconciliation;
        private readonly MonthlyBAExcelService monthlyBaExcelService;
        private readonly ExcelMarginService excelMarginService;

        private const int StartRowIndex = 3;

        public int LastRowIndex
        {
            get; set;
        }

        public int LastAdjustmentRowIndex
        {
            get; set;
        }

        public MonthlyExcelDHWLServiceImp(
            MonthlyDataDHWLService dataDhwlService,
            DateTimeServiceProvider dateService,
            AdjustmentDataService adjustmentService,
            MonthlyReconciliationExcelService reconciliation,
            MonthlyBAExcelService monthlyBaExcelService,
            ExcelMarginService excelMarginService)
        {
            this.dataDhwlService = dataDhwlService;
            this.dateService = dateService;
            this.adjustmentService = adjustmentService;
            this.monthlyBaExcelService = monthlyBaExcelService;
            this.reconciliation = reconciliation;
            this.excelMarginService = excelMarginService;
        }

        public string Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBa, int reportId)
        {
            var dataDhwl = dataDhwlService.CalTotalByProduct(monthlyRawData);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-Letstalk.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Letstalk"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            var marginSheet = excelPackage.Workbook.Worksheets.Add("Monthly Margin");
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(template);
            RenderDataGroup1("B3", dataDhwl, template, formatTpl);
            RenderDataGroup2("I3", dataDhwl, template, formatTpl);
            RenderDataGroup3("P3", dataDhwl, template, formatTpl);
            RenderGrandTotal(dataDhwl, template, formatTpl);

            reportId++;
            RenderAdjustment(monthlyRawData, template);
            RenderReconciliation(dataReconciliation);
            RenderBa(dataBa);
            RenderMtdMargin(monthlyRawData, marginSheet, reportId);

            return Save(template);
        }

        private void RenderDataGroup1(string loadDataAtCell, List<MonthlyDHWLModel> dataDhwl, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var productGroup1 = new List<string> { "Sportsbook", "Live Casino" };
            var dataGroup1 = dataDhwl.Where(s => productGroup1.Contains(s.ProductGroup)).ToList();
            var startCol = 2;
            var endCol = 3;
            RenderData(loadDataAtCell, dataGroup1, template, startCol, endCol);
            FormatData(dataGroup1, template, formatTpl, startCol, endCol);
        }

        private void RenderDataGroup2(string loadDataAtCell, List<MonthlyDHWLModel> dataDhwl, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var productGroup2 = new List<string> { "RNG Casino" };
            var dataGroup2 = dataDhwl.Where(s => productGroup2.Contains(s.ProductGroup)).ToList();
            var startCol = 9;
            var endCol = 10;
            RenderData(loadDataAtCell, dataGroup2, template, startCol, endCol);
            FormatData(dataGroup2, template, formatTpl, startCol, endCol);
        }

        private void RenderDataGroup3(string loadDataAtCell, List<MonthlyDHWLModel> dataDhwl, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var productGroup3 = new List<string> { "Lottery/Keno", "Others" };
            var dataGroup3 = dataDhwl.Where(s => productGroup3.Contains(s.ProductGroup)).ToList();
            var startCol = 16;
            var endCol = 17;
            RenderData(loadDataAtCell, dataGroup3, template, startCol, endCol);
            FormatData(dataGroup3, template, formatTpl, startCol, endCol);

            var lastRowIndex = dataGroup3.Count + 3;
            template.Cells[lastRowIndex, 17].Value = "ReportID #02";
            template.Cells[lastRowIndex, 17].StyleID = formatTpl.Cells["D2"].StyleID;
        }

        private void RenderGrandTotal(List<MonthlyDHWLModel> dataDhwl, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var productGroup4 = new List<string> { "Total" };
            var dataGroup4 = dataDhwl.Where(s => productGroup4.Contains(s.ProductGroup)).ToList();

            var lastRow = template.Dimension.End.Row + 1;
            var templateRow = 6;
            template.Row(lastRow).Height = 5;

            dataGroup4.ForEach(row =>
            {
                lastRow++;
                templateRow++;
                template.Cells[lastRow, 2, lastRow, 9].Merge = true;
                template.Cells[lastRow, 2, lastRow, 9].StyleID = formatTpl.Cells[$"B{templateRow}"].StyleID;
                template.Cells[lastRow, 2, lastRow, 9].Value = row.ProductShortName;
                template.Cells[lastRow, 10, lastRow, 17].Merge = true;
                template.Cells[lastRow, 10, lastRow, 17].StyleID = formatTpl.Cells[$"C{templateRow}"].StyleID;
                template.Cells[lastRow, 10, lastRow, 17].Value = row.WinLossMTD;
                template.Row(lastRow).Height = formatTpl.Row(templateRow).Height;
            });
            ;
        }

        private void RenderMtdMargin(DataTable monthlyRawData, ExcelWorksheet marginSheet, int reportId)
        {
            var mtdMarginExcelPath = excelMarginService.Export(monthlyRawData, false, reportId);
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

        private static void RenderData(string loadDataAtCell, List<MonthlyDHWLModel> data, ExcelWorksheet template, int startCol, int endCol)
        {
            template.Cells[loadDataAtCell].LoadFromCollection(data, false);

            var maxRowIndex = data.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (int columnIndex = startCol; columnIndex <= endCol; columnIndex++)
            {
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        private static void FormatData(List<MonthlyDHWLModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl, int startCol, int endCol)
        {
            var starIndexGroup = StartRowIndex;
            var endIndexGroup = starIndexGroup;
            var isStart = false;

            template.Cells[starIndexGroup, startCol].StyleID = formatTpl.Cells["B5"].StyleID;
            template.Cells[starIndexGroup, endCol].StyleID = formatTpl.Cells["C5"].StyleID;

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
                    template.Cells[starIndexGroup, startCol].StyleID = formatTpl.Cells["B5"].StyleID;
                    template.Cells[starIndexGroup, endCol].StyleID = formatTpl.Cells["C5"].StyleID;
                });
        }

        private void RenderAdjustment(DataTable monthlyRawData, ExcelWorksheet template)
        {
            var toDate = dateService.DateTime.ToDate;
            var dataAjustment = adjustmentService.CalcData(monthlyRawData);

            template.Cells["W2"].Value = $"Explanation for Adjustment - {toDate:MMM yyyy}";
            template.Cells["W4"].LoadFromCollection(dataAjustment, false);

            LastRowIndex = template.Dimension.End.Row;
            LastAdjustmentRowIndex = 8;
        }

        private string Save(ExcelWorksheet template)
        {
            var hideCols = new List<int> { 4, 5, 6, 7, 11, 12, 13, 14, 18, 19, 20, 21 };
            hideCols.ForEach(col =>
            {
                template.Column(col).Hidden = true;
            });

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
            var toDate = dateService.DateTime.ToDate;
            template.Cells["C2"].Value = $"{toDate:MMM-yyyy}";
            template.Cells["J2"].Value = $"{toDate:MMM-yyyy}";
            template.Cells["Q2"].Value = $"{toDate:MMM-yyyy}";
        }

        private void RenderReconciliation(DataTable dataConciliation)
        {
            var tplReconciliation = excelPackage.Workbook.Worksheets["Rebate & Discount"];
            var date = dateService.DateTime.ToDate;
            reconciliation.RenderReconciliation(tplReconciliation, dataConciliation, date);
        }

        private void RenderBa(DataTable dataBa)
        {
            var tplBa = excelPackage.Workbook.Worksheets["BA"];
            var date = dateService.DateTime.ToDate;
            monthlyBaExcelService.RenderBA(tplBa, dataBa, date);
        }
    }
}