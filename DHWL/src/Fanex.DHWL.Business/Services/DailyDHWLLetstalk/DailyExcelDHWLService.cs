using Fanex.DHWL.Business.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services.DailyDHWLLetstalk
{
    public interface DailyExcelDHWLService
    {
        string Export(DataTable dailyRawData, List<MarginModel> grandTotalForDaily, List<MarginModel> grandTotalForMtd);

        int LastRowIndex { get; set; }
    }

    public class DailyExcelDHWLServiceImp : DailyExcelDHWLService
    {
        private readonly DailyDataDHWLService dataDhwlService;
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private const int StartRowIndex = 4;

        public int LastRowIndex
        {
            get; set;
        }

        public DailyExcelDHWLServiceImp(
            DailyDataDHWLService dataDhwlService,
            DateTimeServiceProvider dateService)
        {
            this.dataDhwlService = dataDhwlService;
            this.dateService = dateService;
        }

        public string Export(DataTable dailyRawData, List<MarginModel> grandTotalForDaily, List<MarginModel> grandTotalForMtd)
        {
            var dataDhwl = dataDhwlService.CalTotalByProduct(dailyRawData);

            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-Letstalk.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["Letstalk"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            excelPackage.Workbook.Worksheets.Delete(formatTpl);

            RenderTitle(dailyTemplate);
            RenderDataGroup1("B4", dataDhwl, dailyTemplate, formatTpl);
            RenderDataGroup2("H4", dataDhwl, dailyTemplate, formatTpl);
            RenderDataGroup3("N4", dataDhwl, dailyTemplate, formatTpl);

            RenderCreditTotal(dailyTemplate, formatTpl, grandTotalForDaily, grandTotalForMtd);
            RenderDepositTotal(dailyTemplate, formatTpl, grandTotalForDaily, grandTotalForMtd);
            RenderGrandTotal(dailyTemplate, formatTpl, grandTotalForDaily, grandTotalForMtd);

            return Save(dailyTemplate);
        }

        private void RenderCreditTotal(
            ExcelWorksheet dailyTemplate,
            ExcelWorksheet formatTpl,
            List<MarginModel> grandTotalForDaily,
            List<MarginModel> grandTotalForMtd)
        {
            var endRow = dailyTemplate.Dimension.End.Row + 2;
            LastRowIndex = endRow;
            formatTpl.Cells["B12:P12"].Copy(dailyTemplate.Cells[endRow, 2, endRow, 16]);
            dailyTemplate.Row(endRow - 1).Height = 10;
            dailyTemplate.Row(endRow).Height = 22;

            dailyTemplate.Cells[endRow, 2, endRow, 4].Merge = true;
            dailyTemplate.Cells[endRow, 2, endRow, 4].Value = "Credit Total";
            dailyTemplate.Cells[endRow, 2, endRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var netTotalDaily = grandTotalForDaily.FirstOrDefault(s => s.ProductShortName.Equals("NET TOTAL")) ?? new MarginModel();
            var totalDaily = grandTotalForDaily.FirstOrDefault(s => s.ProductShortName.Equals("Total")) ?? new MarginModel();
            var netTotalMtd = grandTotalForMtd.FirstOrDefault(s => s.ProductShortName.Equals("NET TOTAL")) ?? new MarginModel();
            var totalMtd = grandTotalForMtd.FirstOrDefault(s => s.ProductShortName.Equals("Total")) ?? new MarginModel();
            var dailyWl = totalDaily.TotalWL - totalDaily.LicenseeWL;
            var dailyTo = totalDaily.TotalTO - totalDaily.LicenseeTO;
            var dailyMargin = dailyTo == 0 ? 0 : dailyWl / dailyTo;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Value = netTotalDaily.TotalWL - netTotalDaily.LicenseeWL;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Merge = true;
            dailyTemplate.Cells[endRow, 10, endRow, 10].Value = dailyMargin;

            var mtdWl = totalMtd.TotalWL - totalMtd.LicenseeWL;
            var mtdTo = totalMtd.TotalTO - totalMtd.LicenseeTO;
            var mtdMargin = mtdTo == 0 ? 0 : mtdWl / mtdTo;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Value = netTotalMtd.TotalWL - netTotalMtd.LicenseeWL;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Merge = true;
            dailyTemplate.Cells[endRow, 16, endRow, 16].Value = mtdMargin;
        }

        private void RenderDepositTotal(
            ExcelWorksheet dailyTemplate,
            ExcelWorksheet formatTpl,
            List<MarginModel> grandTotalForDaily,
            List<MarginModel> grandTotalForMtd)
        {
            var endRow = dailyTemplate.Dimension.End.Row + 1;
            LastRowIndex = endRow;
            formatTpl.Cells["B13:P13"].Copy(dailyTemplate.Cells[endRow, 2, endRow, 16]);
            dailyTemplate.Row(endRow).Height = 22;

            dailyTemplate.Cells[endRow, 2, endRow, 4].Merge = true;
            dailyTemplate.Cells[endRow, 2, endRow, 4].Value = "Licensee Total";
            dailyTemplate.Cells[endRow, 2, endRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var netTotalDaily = grandTotalForDaily.FirstOrDefault(s => s.ProductShortName.Equals("NET TOTAL")) ?? new MarginModel();
            var totalDaily = grandTotalForDaily.FirstOrDefault(s => s.ProductShortName.Equals("Total")) ?? new MarginModel();
            var netTotalMtd = grandTotalForMtd.FirstOrDefault(s => s.ProductShortName.Equals("NET TOTAL")) ?? new MarginModel();
            var totalMtd = grandTotalForMtd.FirstOrDefault(s => s.ProductShortName.Equals("Total")) ?? new MarginModel();
            var dailyWl = totalDaily.LicenseeWL;
            var dailyTo = totalDaily.LicenseeTO;
            var dailyMargin = dailyTo == 0 ? 0 : dailyWl / dailyTo;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Value = netTotalDaily.LicenseeWL;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Merge = true;
            dailyTemplate.Cells[endRow, 10, endRow, 10].Value = dailyMargin;

            var mtdWl = totalMtd.LicenseeWL;
            var mtdTo = totalMtd.LicenseeTO;
            var mtdMargin = mtdTo == 0 ? 0 : mtdWl / mtdTo;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Value = netTotalMtd.LicenseeWL;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Merge = true;
            dailyTemplate.Cells[endRow, 16, endRow, 16].Value = mtdMargin;
        }

        private void RenderGrandTotal(
            ExcelWorksheet dailyTemplate,
            ExcelWorksheet formatTpl,
            List<MarginModel> grandTotalForDaily,
            List<MarginModel> grandTotalForMtd)
        {
            var endRow = dailyTemplate.Dimension.End.Row + 1;
            LastRowIndex = endRow;
            formatTpl.Cells["B14:P14"].Copy(dailyTemplate.Cells[endRow, 2, endRow, 16]);
            dailyTemplate.Row(endRow).Height = 25;

            dailyTemplate.Cells[endRow, 2, endRow, 4].Merge = true;
            dailyTemplate.Cells[endRow, 2, endRow, 4].Value = "Grand Total";
            dailyTemplate.Cells[endRow, 2, endRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var netTotalDaily = grandTotalForDaily.FirstOrDefault(s => s.ProductShortName.Equals("NET TOTAL")) ?? new MarginModel();
            var totalDaily = grandTotalForDaily.FirstOrDefault(s => s.ProductShortName.Equals("Total")) ?? new MarginModel();
            var netTotalMtd = grandTotalForMtd.FirstOrDefault(s => s.ProductShortName.Equals("NET TOTAL")) ?? new MarginModel();
            var totalMtd = grandTotalForMtd.FirstOrDefault(s => s.ProductShortName.Equals("Total")) ?? new MarginModel();

            dailyTemplate.Cells[endRow, 5, endRow, 9].Value = netTotalDaily.TotalWL;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dailyTemplate.Cells[endRow, 5, endRow, 9].Merge = true;
            dailyTemplate.Cells[endRow, 10, endRow, 10].Value = totalDaily.TotalMargin;

            dailyTemplate.Cells[endRow, 11, endRow, 15].Value = netTotalMtd.TotalWL;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dailyTemplate.Cells[endRow, 11, endRow, 15].Merge = true;
            dailyTemplate.Cells[endRow, 16, endRow, 16].Value = totalMtd.TotalMargin;
        }

        private void RenderDataGroup1(string loadDataAtCell, List<DailyDHWLModel> dataDhwl, ExcelWorksheet dailyTemplate, ExcelWorksheet formatTpl)
        {
            var productGroup1 = new List<string> { "Sportsbook", "Live Casino" };
            var dataGroup1 = dataDhwl.Where(s => productGroup1.Contains(s.ProductGroup)).ToList();

            const int startCol = 2;
            const int endCol = 4;
            RenderData(loadDataAtCell, dataGroup1, dailyTemplate, startCol, endCol);
            FormatData(dataGroup1, dailyTemplate, formatTpl, startCol, endCol);
        }

        private void RenderDataGroup2(string loadDataAtCell, List<DailyDHWLModel> dataDhwl, ExcelWorksheet dailyTemplate, ExcelWorksheet formatTpl)
        {
            var productGroup2 = new List<string> { "RNG Casino" };
            var dataGroup2 = dataDhwl.Where(s => productGroup2.Contains(s.ProductGroup)).ToList();

            const int startCol = 8;
            const int endCol = 10;
            RenderData(loadDataAtCell, dataGroup2, dailyTemplate, startCol, endCol);
            FormatData(dataGroup2, dailyTemplate, formatTpl, startCol, endCol);
        }

        private void RenderDataGroup3(string loadDataAtCell, List<DailyDHWLModel> dataDhwl, ExcelWorksheet dailyTemplate, ExcelWorksheet formatTpl)
        {
            var productGroup3 = new List<string> { "Lottery/Keno", "Others", "Gaming Platform" };
            var dataGroup3 = dataDhwl.Where(s => productGroup3.Contains(s.ProductGroup)).ToList();

            const int startCol = 14;
            const int endCol = 16;
            RenderData(loadDataAtCell, dataGroup3, dailyTemplate, startCol, endCol);
            FormatData(dataGroup3, dailyTemplate, formatTpl, startCol, endCol);
            var lastRowIndex = dataGroup3.Count + 4;
            dailyTemplate.Cells[lastRowIndex, 16].Value = "ReportID #01";
            dailyTemplate.Cells[lastRowIndex, 16].StyleID = formatTpl.Cells["B10"].StyleID;
        }

        private static void RenderData(string loadDataAtCell, List<DailyDHWLModel> data, ExcelWorksheet template, int startCol, int endCol)
        {
            template.Cells[loadDataAtCell].LoadFromCollection(data, false);

            var maxRowIndex = data.Count + StartRowIndex - 1;

            for (var rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 19.5;
            }

            for (var columnIndex = startCol; columnIndex <= endCol; columnIndex++)
            {
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }
        }

        private void FormatData(List<DailyDHWLModel> data, ExcelWorksheet template, ExcelWorksheet formatTpl, int startCol, int endCol)
        {
            var starIndexGroup = StartRowIndex;
            var endIndexGroup = starIndexGroup;
            var isStart = false;

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

                    template.Cells[starIndexGroup, startCol].StyleID = formatTpl.Cells["B6"].StyleID;
                    template.Cells[starIndexGroup, startCol + 1, starIndexGroup, endCol].StyleID = formatTpl.Cells["D6"].StyleID;

                    endIndexGroup = starIndexGroup + productGroup.Count() - 1;
                });
        }

        private string Save(ExcelWorksheet dailyTemplate)
        {
            var delCols = new List<int> { 6, 7, 12, 13, 18, 19 };
            delCols.ForEach(col =>
            {
                dailyTemplate.Column(col).Hidden = true;
            });

            var date = dateService.DateTime.ToDate;
            var fileName = $"Daily-DHWL-{date:yyyy_MM_dd}.xlsx";

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
            template.Cells["C3"].Value = $"{toDate:dd-MMM-yyyy}";
            template.Cells["I3"].Value = $"{toDate:dd-MMM-yyyy}";
            template.Cells["O3"].Value = $"{toDate:dd-MMM-yyyy}";
        }
    }
}