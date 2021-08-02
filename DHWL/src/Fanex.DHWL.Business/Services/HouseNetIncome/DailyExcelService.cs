using Fanex.DHWL.Business.Models;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DailyExcelService
    {
        string Export(DataTable dailyRawData);
    }

    public class DailyExcelServiceImp : DailyExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;
        private readonly DataHouseIncomeService dataHouseIncomeService;
        private readonly DataService dataService;
        private readonly DataLastMonthService dataLastMonthService;
        private readonly ExcelRawService excelRawService;

        private const int StartRowIndex = 7;

        public DailyExcelServiceImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth,
            DataHouseIncomeService dataHouseIncomeService,
            DataService dataService,
            DataLastMonthService dataLastMonthService,
            ExcelRawService excelRawService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
            this.dataHouseIncomeService = dataHouseIncomeService;
            this.dataService = dataService;
            this.dataLastMonthService = dataLastMonthService;
            this.excelRawService = excelRawService;
        }

        public string Export(DataTable dailyRawData)
        {
            var dataForExcel = dataHouseIncomeService.CalTotalByProductForExcel(dailyRawData);
            excelPackage = new ExcelPackage(new FileInfo($"Templates/DHWL-For-eMail.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["House Income"];
            var lastMonthTemplate = excelPackage.Workbook.Worksheets["Last-Month"];
            var formatTpl = excelPackage.Workbook.Worksheets["Format"];
            excelPackage.Workbook.Worksheets.Delete(formatTpl);
            excelPackage.Workbook.Worksheets["House Income"].Select();

            RenderDailyTitle(dailyTemplate);
            RenderDailyData(dataForExcel, dailyTemplate);
            FormatDailyData(dataForExcel, dailyTemplate, formatTpl);

            RenderLastMonthTitle(lastMonthTemplate);
            var lastMonthRawData = ProcessLastMonth();
            RenderLastMonthData(lastMonthRawData, lastMonthTemplate, formatTpl);

            return Save();
        }

        private DataTable ProcessLastMonth()
        {
            var lastMonthRawData = dataService.QueryLastMonthData();
            excelRawService.Export(lastMonthRawData, "Monthly", lastMonth.FromDate, lastMonth.ToDate);
            return dataLastMonthService.CalcData(lastMonthRawData);
        }

        private void RenderDailyData(DataTable dataTable, ExcelWorksheet template)
        {
            template.Cells["B7"].LoadFromDataTable(dataTable, false);

            var maxRowIndex = dataTable.Rows.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 14.4;
            }

            for (int columnIndex = 2; columnIndex <= 3; columnIndex++)
            {
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }

            for (int columnIndex = 4; columnIndex <= dataTable.Columns.Count + 1; columnIndex++)
            {
                if (columnIndex > 4)
                {
                    template.Cells[6, columnIndex].Value = dataTable.Columns[columnIndex - 2].ColumnName;
                }

                template.Cells[5, columnIndex, 6, columnIndex].StyleID = template.Cells[StartRowIndex - 1, 4].StyleID;
                template.Cells[StartRowIndex - 1, columnIndex, StartRowIndex - 1, columnIndex].StyleID = template.Cells[StartRowIndex - 1, 4].StyleID;
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, 4].StyleID;
                template.Column(columnIndex).AutoFit();
            }

            template.Cells[5, 5, 5, dataTable.Columns.Count + 1].Value = $"{dateService.DateTime.ToDate:MMM yyyy}";
            template.Cells[5, 5, 5, dataTable.Columns.Count + 1].Merge = true;
        }

        private void FormatDailyData(DataTable dataTable, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            var startProductRowIndex = StartRowIndex;
            var endProductRowIndex = 0;
            dataTable.AsEnumerable().GroupBy(product => new
            {
                ProductName = product[ColumnName.ProductName]
            }).ToList()
            .ForEach(p =>
            {
                var s = p.Key.ProductName;
                if (endProductRowIndex == 0)
                {
                    startProductRowIndex = StartRowIndex;
                }
                else
                {
                    startProductRowIndex = endProductRowIndex + 1;
                }

                endProductRowIndex = startProductRowIndex + p.Count() - 1;
                template.Cells[startProductRowIndex, 2, endProductRowIndex, 2].Merge = true;
                template.Cells[endProductRowIndex, 3, endProductRowIndex, dataTable.Columns.Count + 1].StyleID = formatTpl.Cells[2, 3].StyleID;

                for (int rowIndex = startProductRowIndex; rowIndex < endProductRowIndex; rowIndex++)
                {
                    template.Row(rowIndex).OutlineLevel = 1;
                    template.Row(rowIndex).Collapsed = true;
                }
            });

            for (int columnIndex = 4; columnIndex <= dataTable.Columns.Count; columnIndex++)
            {
                template.Column(columnIndex).OutlineLevel = 1;
                template.Column(columnIndex).Collapsed = true;
            }

            template.Cells[endProductRowIndex, 2, endProductRowIndex, dataTable.Columns.Count + 1].StyleID = formatTpl.Cells[3, 3].StyleID;
        }

        private string Save()
        {
            var date = dateService.DateTime.ToDate;
            var fileName = $"House_Net_Income_On_{date:yyyy_MM_dd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }

        private void RenderDailyTitle(ExcelWorksheet template)
        {
            var now = DateTime.Now;
            var fromdate = dateService.DateTime.FromDate;
            var todate = dateService.DateTime.ToDate;

            template.Cells["B2"].Value = $"House Month-to-Date Net Income Report From {fromdate:yyyy-MM-dd} To {todate:yyyy-MM-dd}";
            template.Cells["B3"].Value = $"Generated by it on {now}";
        }

        private void RenderLastMonthTitle(ExcelWorksheet template)
        {
            var now = DateTime.Now;
            var fromdate = lastMonth.FromDate;

            template.Cells["B2"].Value = $"House Month-to-Date Net Income Report On {fromdate:MMMM yyyy}";
            template.Cells["B3"].Value = $"Generated by it on {now}";
            template.Cells["C5"].Value = $"{fromdate:MMM yyy}";
            template.Name = $"{fromdate:MMM yyy}";
        }

        private void RenderLastMonthData(DataTable dataTable, ExcelWorksheet template, ExcelWorksheet formatTpl)
        {
            template.Cells["B7"].LoadFromDataTable(dataTable, false);
            var maxRowIndex = dataTable.Rows.Count + StartRowIndex - 1;

            for (int rowIndex = StartRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 14.4;
            }

            for (int columnIndex = 2; columnIndex <= 3; columnIndex++)
            {
                template.Cells[StartRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[StartRowIndex, columnIndex].StyleID;
            }

            for (int columnIndex = 2; columnIndex <= 3; columnIndex++)
            {
                int rowIndexTpl = 4;
                for (int rowIndex = maxRowIndex - 2; rowIndex <= maxRowIndex; rowIndex++)
                {
                    template.Cells[rowIndex, columnIndex].StyleID = formatTpl.Cells[rowIndexTpl, columnIndex].StyleID;
                    rowIndexTpl++;
                }
            }

            template.Cells[maxRowIndex + 2, 2].Value = "* Adjustments are due to monthly rebate & discount given to licensee, and minimum fee taken from licensee.";
        }
    }
}