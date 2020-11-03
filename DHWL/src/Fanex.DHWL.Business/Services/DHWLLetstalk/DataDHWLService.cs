using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.DHWL.Business.Services.DHWLLetstalk
{
    public interface DataDHWLService
    {
        List<DHWLModel> CalTotalByProduct(DataTable source);
    }

    public class DataDHWLServiceImp : DataDHWLService
    {
        private readonly DataService dataService;

        public DataDHWLServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

        public List<DHWLModel> CalTotalByProduct(DataTable source)
        {
            var result = new List<DHWLModel>();
            var yesterdayColumnIndex = source.Columns.Count - 1;
            var mtdColumnIndex = 9;
            source.AsEnumerable()
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

            return result;
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

            var cn88Data = dataService.QueryCN88();
            var c8 = new DHWLModel
            {
                ProductShortName = "Total (C8)",
                ProductGroup = "Total",
                OrderByProduct = 1000,
                WinLossYesterday = result.Sum(s => s.WinLossYesterday),
                WinLossMTD = result.Sum(s => s.WinLossMTD)
            };

            result.Add(total);
        }
    }
}