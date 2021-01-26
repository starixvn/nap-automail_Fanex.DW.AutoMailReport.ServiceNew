using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk
{
    public interface MonthlyDataDHWLService
    {
        List<MonthlyDHWLModel> CalTotalByProduct(DataTable monthlyRawData);
    }

    public class MonthlyDataDHWLServiceImp : MonthlyDataDHWLService
    {
        public List<MonthlyDHWLModel> CalTotalByProduct(DataTable monthlyRawData)
        {
            var result = new List<MonthlyDHWLModel>();

            monthlyRawData.AsEnumerable()
                .Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                .GroupBy(g => new
                {
                    ProductShortName = g[ColumnName.ProductShortName].ToString(),
                    ProductGroup = g[ColumnName.ProductGroup].ToString(),
                    OrderByProduct = Converter.To<int>(g[ColumnName.OrderByProduct])
                })
                .ToList()
                .ForEach(product =>
                {
                    var row = new MonthlyDHWLModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        WinLossMonthly = product.Sum(s => Converter.To<decimal>(s[ColumnName.Monthly])),
                        WinLossMTD = product.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]))
                    };

                    result.Add(row);
                });

            CalcTotal(result);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private void CalcTotal(List<MonthlyDHWLModel> result)
        {
            var winLossMonthly = result.Sum(s => s.WinLossMonthly);
            var winLossMTD = result.Sum(s => s.WinLossMTD);
            var total = new MonthlyDHWLModel
            {
                ProductShortName = "Total",
                ProductGroup = "Total",
                OrderByProduct = 1000,
                WinLossMTD = winLossMTD
            };

            var adj = new MonthlyDHWLModel
            {
                ProductShortName = "Adjustment",
                ProductGroup = "Adjustment",
                OrderByProduct = 1001,
                WinLossMTD = winLossMonthly - winLossMTD,
            };

            var netIncome = new MonthlyDHWLModel
            {
                ProductShortName = "Net Income",
                ProductGroup = "Net Income",
                OrderByProduct = 1002,
                WinLossMTD = winLossMonthly,
            };

            result.Add(total);
            result.Add(adj);
            result.Add(netIncome);
        }
    }
}