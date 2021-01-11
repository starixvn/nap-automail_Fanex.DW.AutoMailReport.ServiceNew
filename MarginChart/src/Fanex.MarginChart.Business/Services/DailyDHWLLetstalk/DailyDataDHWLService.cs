using Fanex.Common;
using Fanex.MarginChart.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.MarginChart.Business.Services.DailyDHWLLetstalk
{
    public interface DailyDataDHWLService
    {
        List<DailyDHWLModel> CalTotalByProduct(DataTable dailyRawData);
    }

    public class DailyDataDHWLServiceImp : DailyDataDHWLService
    {
        private readonly DataService dataService;

        public DailyDataDHWLServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

        public List<DailyDHWLModel> CalTotalByProduct(DataTable dailyRawData)
        {
            var result = new List<DailyDHWLModel>();
            var yesterdayColumnIndex = dailyRawData.Columns.Count - 1;

            dailyRawData.AsEnumerable()
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
                    var row = new DailyDHWLModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        WinLossYesterday = product.Sum(s => Converter.To<decimal>(s[yesterdayColumnIndex])),
                        WinLossMTD = product.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]))
                    };

                    result.Add(row);
                });

            CalcTotal(result);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private void CalcTotal(List<DailyDHWLModel> result)
        {
            var total = new DailyDHWLModel
            {
                ProductShortName = "Total",
                ProductGroup = "Total",
                OrderByProduct = 1000,
                WinLossYesterday = result.Sum(s => s.WinLossYesterday),
                WinLossMTD = result.Sum(s => s.WinLossMTD)
            };

            result.Add(total);
        }
    }
}