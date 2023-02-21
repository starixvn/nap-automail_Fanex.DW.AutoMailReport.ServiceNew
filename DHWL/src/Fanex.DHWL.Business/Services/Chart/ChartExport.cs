using Fanex.DHWL.Business.Models;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface ChartExport
    {
        string Export(DataTable rawDataAllProducts, DataTable previousRawData);
    }

    public class ChartExportImp : ChartExport
    {
        private readonly ChartDataService calculatorService;
        private readonly ChartService chartService;

        public ChartExportImp(
            ChartDataService calculatorService,
            ChartService chartService)
        {
            this.calculatorService = calculatorService;
            this.chartService = chartService;
        }

        public string Export(DataTable rawDataAllProducts, DataTable previousRawData)
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

            var chartPath = chartService.ExportDailyChart(sbFigure, totalFigure, mtdFigure);

            return chartPath;
        }
    }
}