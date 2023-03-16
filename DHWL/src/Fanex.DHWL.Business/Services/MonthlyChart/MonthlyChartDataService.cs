using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyChart
{
    public interface MonthlyChartDataService
    {
        Dictionary<int, double> CalculateSportsbookTotalForDaily(DataTable rawDataAllProducts);

        Dictionary<int, double> CalculateGrandTotalForMonthly(DataTable rawDataAllProducts, string year);

        Dictionary<int, double> CalcTotal(Dictionary<int, double> value1, Dictionary<int, double> value2);
    }

    public class MonthlyChartDataServiceImp : MonthlyChartDataService
    {
        public Dictionary<int, double> CalculateGrandTotalForMonthly(DataTable rawDataAllProducts, string year)
        {
            var dataByYear = rawDataAllProducts.AsEnumerable()
                .Where(s => s["MonthOfYear"].ToString().Contains(year))
                .CopyToDataTable();

            var resultByMonth = new Dictionary<int, double>();
            dataByYear.AsEnumerable().GroupBy(g => new { MonthOfYear = g["MonthOfYear"] }).ToList()
                .ForEach(month =>
                {
                    var dataByMonth = month.CopyToDataTable();
                    var resultByDate = new Dictionary<int, double>();
                    for (var columnIndex = ColumnIndex.StartDateColumnIndex + 1; columnIndex < dataByMonth.Columns.Count; columnIndex++)
                    {
                        var totalByDate = dataByMonth.AsEnumerable().Sum(s => Converter.To<double>(s[columnIndex]));
                        var date = Converter.To<int>(dataByYear.Columns[columnIndex].ColumnName);
                        resultByDate.Add(date, totalByDate);
                    }

                    var m = Converter.To<int>(month.Key.MonthOfYear.ToString().Split('-')[1]);
                    var totalByMonth = resultByDate.Sum(s => s.Value);
                    resultByMonth.Add(m, totalByMonth);
                });

            return resultByMonth;
        }

        public Dictionary<int, double> CalcTotal(Dictionary<int, double> value1, Dictionary<int, double> value2)
        {
            var result = new Dictionary<int, double>();
            foreach (var item in value1)
            {
                result.Add(item.Key, value1[item.Key] + value2[item.Key]);
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