using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyChart
{
    public interface MonthlyChartDataService
    {
        Dictionary<int, double> CalculateGrandTotalForMonthly(DataTable rawDataAllProducts, string year, DepositType depositType);
    }

    public class MonthlyChartDataServiceImp : MonthlyChartDataService
    {
        public Dictionary<int, double> CalculateGrandTotalForMonthly(DataTable rawDataAllProducts, string year, DepositType depositType)
        {
            var dataByYear = rawDataAllProducts.Clone();
            if (depositType == DepositType.All)
            {
                var source = rawDataAllProducts.AsEnumerable()
                    .Where(s => s["MonthOfYear"].ToString().Contains(year));
                if (source.Any())
                {
                    dataByYear = source.CopyToDataTable();
                }
            }
            else
            {
                var source = rawDataAllProducts.AsEnumerable()
                    .Where(s => s["MonthOfYear"].ToString().Contains(year) &&
                                Converter.To<int>(s["IsDeposit"]) == Converter.To<int>(depositType));
                if (source.Any())
                {
                    dataByYear = source.CopyToDataTable();
                }
            }

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
    }
}