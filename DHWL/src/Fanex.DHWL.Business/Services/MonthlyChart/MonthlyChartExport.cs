using Fanex.DHWL.Business.Models;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyChart
{
    public interface MonthlyChartExport
    {
        string Export(DataTable rawDataAllProducts);
    }

    public class MonthlyChartExportImp : MonthlyChartExport
    {
        private readonly MonthlyChartDataService calculatorService;
        private readonly MonthlyChartService chartService;
        private readonly BaseDataTable baseDataTable;

        public MonthlyChartExportImp(
            MonthlyChartDataService calculatorService,
            MonthlyChartService chartService,
            BaseDataTable baseDataTable)
        {
            this.calculatorService = calculatorService;
            this.chartService = chartService;
            this.baseDataTable = baseDataTable;
        }

        public string Export(DataTable rawDataAllProducts)
        {
            var dailyResult = rawDataAllProducts.Rows.Count > 0
                ? rawDataAllProducts.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : rawDataAllProducts;

            var credit = calculatorService.CalculateGrandTotalForMonthly(dailyResult, "2022");
            var lic = calculatorService.CalculateGrandTotalForMonthly(dailyResult, "2023");
            var total = calculatorService.CalcTotal(credit, lic);

            //var previousTotalFigure = calculatorService.CalculateGrandTotalForDaily(previousResult);

            var mtd = lic.Sum(s => s.Value);
            var previousMtd = 0;//previousTotalFigure.Sum(s => s.Value);
            var mtdFigure = new ChartMtdFigureModel
            {
                Mtd = mtd,
                PreviousMtd = previousMtd
            };
            //   CalcMargin(rawDataAllProducts, previousRawData, mtdFigure);
            var chartPath = chartService.ExportMonthlyChart(credit, lic, total, mtdFigure);

            return chartPath;
        }

        private void CalcMargin(DataTable rawDataAllProducts, DataTable previousRawData, ChartMtdFigureModel mtdFigure, string year)
        {
            var rawDataAllProductsMtd = baseDataTable.CloneWithoutIgnoreFigureRows(rawDataAllProducts);
            var previousRawDataMtd = baseDataTable.CloneWithoutIgnoreFigureRows(previousRawData);

            var dailyResultMtd = rawDataAllProductsMtd.Rows.Count > 0
                ? rawDataAllProductsMtd.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : rawDataAllProductsMtd;
            var previousResultMtd = previousRawDataMtd.Rows.Count > 0
                ? previousRawDataMtd.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : previousRawDataMtd;

            var dailyResultTo = rawDataAllProductsMtd.Rows.Count > 0
                ? rawDataAllProductsMtd.AsEnumerable().Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : rawDataAllProductsMtd;
            var previousResultTo = previousRawDataMtd.Rows.Count > 0
                ? previousRawDataMtd.AsEnumerable().Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : previousRawDataMtd;

            var totalFigureMtd = calculatorService.CalculateGrandTotalForMonthly(dailyResultMtd, year);
            var previousTotalFigureMtd = calculatorService.CalculateGrandTotalForMonthly(previousResultMtd, year);

            var totalFigureTo = calculatorService.CalculateGrandTotalForMonthly(dailyResultTo, year);
            var previousTotalFigureTo = calculatorService.CalculateGrandTotalForMonthly(previousResultTo, year);

            var mtd = totalFigureMtd.Sum(s => s.Value);
            var previousMtd = previousTotalFigureMtd.Sum(s => s.Value);

            var to = totalFigureTo.Sum(s => s.Value);
            var previousTo = previousTotalFigureTo.Sum(s => s.Value);

            mtdFigure.Margin = to == 0 ? 0 : mtd / to;
            mtdFigure.PreviousMargin = previousTo == 0 ? 0 : previousMtd / previousTo;
        }
    }
}