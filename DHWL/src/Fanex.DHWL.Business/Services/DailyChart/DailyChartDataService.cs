using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.DailyChart
{
    public interface DailyChartDataService
    {
        Dictionary<int, double> CalculateSportsbookTotalForDaily(DataTable rawDataAllProducts);

        Dictionary<int, double> CalculateGrandTotalForDaily(DataTable rawDataAllProducts);
    }

    public class DailyChartDataServiceImp : DailyChartDataService
    {
        public Dictionary<int, double> CalculateGrandTotalForDaily(DataTable rawDataAllProducts)
        {
            var result = new Dictionary<int, double>();
            for (var columnIndex = ColumnIndex.StartDateColumnIndex; columnIndex < rawDataAllProducts.Columns.Count; columnIndex++)
            {
                var grandTotal = rawDataAllProducts.AsEnumerable().Sum(s => Converter.To<double>(s[columnIndex]));
                var date = Converter.To<int>(rawDataAllProducts.Columns[columnIndex].ColumnName);
                result.Add(date, grandTotal);
            }

            return result;
        }

        public Dictionary<int, double> CalculateSportsbookTotalForDaily(DataTable rawDataAllProducts)
        {
            var result = new Dictionary<int, double>();
            for (var columnIndex = ColumnIndex.StartDateColumnIndex; columnIndex < rawDataAllProducts.Columns.Count; columnIndex++)
            {
                var total = rawDataAllProducts.AsEnumerable()
                    .Where(s => s[ColumnName.ProductName].ToString().Equals("Sportsbook"))
                    .Sum(s => Converter.To<double>(s[columnIndex]));
                var date = Converter.To<int>(rawDataAllProducts.Columns[columnIndex].ColumnName);
                result.Add(date, total);
            }

            return result;
        }
    }
}