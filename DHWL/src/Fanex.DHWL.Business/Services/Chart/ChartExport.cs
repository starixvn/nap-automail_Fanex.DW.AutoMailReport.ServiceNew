using Fanex.DHWL.Business.Models;
using System.Data;
using System.Linq;

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
            var result = rawDataAllProducts.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).CopyToDataTable();
            var sbfigure = calculatorService.CalculateSportsbookTotalForDaily(result);
            var totalFigure = calculatorService.CalculateGrandTotalForDaily(result);
            var chartPath = chartService.ExportDailyChart(sbfigure, totalFigure);

            return chartPath;
        }
    }
}