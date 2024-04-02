using Fanex.DHWL.Business.Models;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.DailyChart
{
    public interface DailyChartExport
    {
        string Export(DataTable rawDataAllProducts, DataTable previousRawData, string title, string chartName);
    }

    public class DailyChartExportImp : DailyChartExport
    {
        private readonly DailyChartDataService calculatorService;
        private readonly DailyChartService chartService;
        private readonly BaseDataTable baseDataTable;

        public DailyChartExportImp(
            DailyChartDataService calculatorService,
            DailyChartService chartService,
            BaseDataTable baseDataTable)
        {
            this.calculatorService = calculatorService;
            this.chartService = chartService;
            this.baseDataTable = baseDataTable;
        }

        public string Export(DataTable rawDataAllProducts, DataTable previousRawData, string title, string chartName)
        {
            var dailyResult = rawDataAllProducts.Rows.Count > 0
                ? rawDataAllProducts.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : rawDataAllProducts;
            var previousResult = previousRawData.Rows.Count > 0
                ? previousRawData.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                : previousRawData;

            var sbFigure = calculatorService.CalculateSportsbookTotalForDaily(dailyResult);
            var totalFigure = calculatorService.CalculateGrandTotalForDaily(dailyResult);
            var previousTotalFigure = calculatorService.CalculateGrandTotalForDaily(previousResult);

            var mtd = totalFigure.Sum(s => s.Value);
            var previousMtd = previousTotalFigure.Sum(s => s.Value);
            var mtdFigure = new ChartMtdFigureModel
            {
                Mtd = mtd,
                PreviousMtd = previousMtd
            };
            CalcMargin(rawDataAllProducts, previousRawData, mtdFigure);
            var chartPath = chartService.ExportDailyChart(sbFigure, totalFigure, mtdFigure, title, chartName);

            return chartPath;
        }

        private void CalcMargin(DataTable rawDataAllProducts, DataTable previousRawData, ChartMtdFigureModel mtdFigure)
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

            var totalFigureMtd = calculatorService.CalculateGrandTotalForDaily(dailyResultMtd);
            var previousTotalFigureMtd = calculatorService.CalculateGrandTotalForDaily(previousResultMtd);

            var totalFigureTo = calculatorService.CalculateGrandTotalForDaily(dailyResultTo);
            var previousTotalFigureTo = calculatorService.CalculateGrandTotalForDaily(previousResultTo);

            var mtd = totalFigureMtd.Sum(s => s.Value);
            var previousMtd = previousTotalFigureMtd.Sum(s => s.Value);

            var to = totalFigureTo.Sum(s => s.Value);
            var previousTo = previousTotalFigureTo.Sum(s => s.Value);

            mtdFigure.Margin = to == 0 ? 0 : mtd / to;
            mtdFigure.PreviousMargin = previousTo == 0 ? 0 : previousMtd / previousTo;
        }
    }
}