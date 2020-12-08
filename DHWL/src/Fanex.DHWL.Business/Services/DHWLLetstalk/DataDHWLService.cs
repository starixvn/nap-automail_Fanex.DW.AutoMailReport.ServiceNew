using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.DHWLLetstalk
{
    public interface DataDHWLService
    {
        List<DHWLModel> CalTotalByProduct(DataTable dailyRawData);
    }

    public class DataDHWLServiceImp : DataDHWLService
    {
        private readonly DataService dataService;

        public DataDHWLServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

        public List<DHWLModel> CalTotalByProduct(DataTable dailyRawData)
        {
            var result = new List<DHWLModel>();
            var yesterdayColumnIndex = dailyRawData.Columns.Count - 1;
            var mtdColumnIndex = 9;
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
                    var row = new DHWLModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        WinLossYesterday = product.Sum(s => Converter.To<decimal>(s[yesterdayColumnIndex])),
                        WinLossMTD = product.Sum(s => Converter.To<decimal>(s[mtdColumnIndex]))
                    };

                    result.Add(row);
                });

            CalcTotal(result);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private void CalcTotal(List<DHWLModel> result)
        {
            var total = new DHWLModel
            {
                ProductShortName = "Total",
                ProductGroup = "Total",
                OrderByProduct = 1000,
                WinLossYesterday = result.Sum(s => s.WinLossYesterday),
                WinLossMTD = result.Sum(s => s.WinLossMTD)
            };

            result.Add(total);

            var cn88Data = dataService.QueryCN88();
            var c8 = CacTotalCN88ByProductCode(cn88Data, "CN88", "Total (C8)", 1001);
            result.Add(c8);

            var tpg = CacTotalCN88ByProductCode(cn88Data, "TPG", "Total TPG (C8)", 1002);
            result.Add(tpg);

            var xpg = CacTotalCN88ByProductCode(cn88Data, "XPG", "Total XPG (C8)", 1003);
            result.Add(xpg);

            var totalWOC8 = new DHWLModel
            {
                ProductShortName = "Total (w/o C8)",
                ProductGroup = "Total",
                OrderByProduct = 1004,
                WinLossYesterday = total.WinLossYesterday - c8.WinLossYesterday,
                WinLossMTD = total.WinLossMTD - c8.WinLossMTD
            };

            result.Add(totalWOC8);
        }

        private DHWLModel CacTotalCN88ByProductCode(DataTable cn88, string productCode, string productShortName, int orderByProduct)
        {
            var beginColumnIndex = 4;
            var lastColumnIndex = cn88.Columns.Count - 1;
            var wl = cn88.AsEnumerable().Where(s => s["ProductCode"].ToString().Equals(productCode))
                .Sum(s => Converter.To<decimal>(s[lastColumnIndex]));
            decimal wlMTD = 0;
            for (var index = beginColumnIndex; index < lastColumnIndex; index++)
            {
                wlMTD += cn88.AsEnumerable().Where(s => s["ProductCode"].ToString().Equals(productCode))
                .Sum(s => Converter.To<decimal>(s[index]));
            }

            return new DHWLModel
            {
                ProductShortName = productShortName,
                OrderByProduct = orderByProduct,
                ProductGroup = "Total",
                WinLossYesterday = wl,
                WinLossMTD = wlMTD
            };
        }
    }
}