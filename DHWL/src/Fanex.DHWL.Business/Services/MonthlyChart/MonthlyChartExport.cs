using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyChart
{
    public interface MonthlyChartExport
    {
        string Export(DataTable rawDataAllProducts, DepositType depositType);
    }

    public class MonthlyChartExportImp : MonthlyChartExport
    {
        private readonly MonthlyChartDataService calculatorService;
        private readonly MonthlyChartService chartService;
        private readonly BaseDataTable baseDataTable;
        private readonly DateTimeServiceProvider dateTimeService;

        public MonthlyChartExportImp(
            MonthlyChartDataService calculatorService,
            MonthlyChartService chartService,
            BaseDataTable baseDataTable,
            DateTimeServiceProvider dateTimeService)
        {
            this.calculatorService = calculatorService;
            this.chartService = chartService;
            this.baseDataTable = baseDataTable;
            this.dateTimeService = dateTimeService;
        }

        public string Export(DataTable rawDataAllProducts, DepositType depositType)
        {
            var wlAfterPay = rawDataAllProducts.Rows.Count > 0
                ? rawDataAllProducts.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : rawDataAllProducts;

            var date = dateTimeService.DateTime.ToDate;
            var lastYear = calculatorService.CalculateGrandTotalForMonthly(wlAfterPay, (date.Year - 1).ToString(), depositType);
            var currentYear = calculatorService.CalculateGrandTotalForMonthly(wlAfterPay, date.Year.ToString(), depositType);

            RefineData(lastYear, currentYear);

            var wlMtdCurrentYear = currentYear.Sum(s => s.Value);
            var wlMtdLastYear = lastYear.Sum(s => s.Value);
            var mtdFigure = new ChartMtdFigureModel
            {
                Mtd = wlMtdCurrentYear,
                PreviousMtd = wlMtdLastYear
            };

            CalcMargin(rawDataAllProducts, mtdFigure, depositType);
            var type = GetDepositType[depositType];
            var chartPath = chartService.ExportMonthlyChart(lastYear, currentYear, mtdFigure, type);

            return chartPath;
        }

        private static void RefineData(Dictionary<int, double> lastYear, Dictionary<int, double> currentYear)
        {
            foreach (var month in lastYear)
            {
                if (!currentYear.ContainsKey(month.Key))
                {
                    currentYear.Add(month.Key, 0);
                }
            }
        }

        private void CalcMargin(DataTable rawDataAllProducts, ChartMtdFigureModel mtdFigure, DepositType depositType)
        {
            var wlAfterPay = rawDataAllProducts.Rows.Count > 0
                ? rawDataAllProducts.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : rawDataAllProducts;

            var date = dateTimeService.DateTime.ToDate;
            var wlBeforePay = baseDataTable.CloneWithoutIgnoreFigureRows(wlAfterPay);
            var to = rawDataAllProducts.AsEnumerable()
                .Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable();
            var wlLastYear = calculatorService.CalculateGrandTotalForMonthly(wlBeforePay, (date.Year - 1).ToString(), depositType);
            var wlCurrentYear = calculatorService.CalculateGrandTotalForMonthly(wlBeforePay, (date.Year).ToString(), depositType);
            var toLastYear = calculatorService.CalculateGrandTotalForMonthly(to, (date.Year - 1).ToString(), depositType);
            var toCurrentYear = calculatorService.CalculateGrandTotalForMonthly(to, date.Year.ToString(), depositType);
            var totalWlLastYear = wlLastYear.Sum(s => s.Value);
            var totalWlCurrentYear = wlCurrentYear.Sum(s => s.Value);
            var totalToLastYear = toLastYear.Sum(s => s.Value);
            var totalToCurrentYear = toCurrentYear.Sum(s => s.Value);
            mtdFigure.PreviousMargin = totalToLastYear == 0 ? 0 : totalWlLastYear / totalToLastYear;
            mtdFigure.Margin = totalToCurrentYear == 0 ? 0 : totalWlCurrentYear / totalToCurrentYear;
        }

        private static Dictionary<DepositType, string> GetDepositType
        {
            get
            {
                var type = new Dictionary<DepositType, string>
                {
                    { DepositType.CreditOnly, "Credit" },
                    { DepositType.DepositOnly, "Licensee" },
                    { DepositType.All, "Total" }
                };

                return type;
            }
        }
    }
}