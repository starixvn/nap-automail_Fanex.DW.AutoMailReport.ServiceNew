using System.Data;

namespace Fanex.DHWL.Business.Services
{
    public interface ChartExport
    {
        string Export(DataTable rawDataAllProducts);
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

        public string Export(DataTable rawDataAllProducts)
        {
            var sbfigure = calculatorService.CalculateSportsbookTotalForDaily(rawDataAllProducts);
            var totalFigure = calculatorService.CalculateGrandTotalForDaily(rawDataAllProducts);
            var cn88Figure = calculatorService.CalculateCN88(rawDataAllProducts);
            var chartPath = chartService.ExportDailyChart(sbfigure, totalFigure, cn88Figure);

            return chartPath;
        }
    }
}