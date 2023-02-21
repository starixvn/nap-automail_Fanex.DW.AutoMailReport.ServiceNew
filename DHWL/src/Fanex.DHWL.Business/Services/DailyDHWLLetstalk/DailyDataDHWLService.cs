using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.DailyDHWLLetstalk
{
    public interface DailyDataDHWLService
    {
        List<DailyDHWLModel> CalTotalByProduct(DataTable dailyRawData);
    }

    public class DailyDataDHWLServiceImp : DailyDataDHWLService
    {
        public List<DailyDHWLModel> CalTotalByProduct(DataTable dailyRawData)
        {
            var result = new List<DailyDHWLModel>();
            var resultSubTotal = new List<DailyDHWLModel>();
            var yesterdayColumnIndex = dailyRawData.Columns.Count - 1;

            CalcYesterdayAndMTDByProduct(dailyRawData, result, yesterdayColumnIndex);
            CalcSubTotalByProduct(result, resultSubTotal);
            CalcTotal(result, resultSubTotal);

            return resultSubTotal.OrderBy(o => o.OrderByProduct).ToList();
        }

        private static void CalcSubTotalByProduct(List<DailyDHWLModel> result, List<DailyDHWLModel> resultSubTotal)
        {
            result.GroupBy(g => g.ProductGroup)
                            .ToList()
                            .ForEach(productGroup =>
                            {
                                resultSubTotal.Add(new DailyDHWLModel
                                {
                                    ProductShortName = productGroup.Key,
                                    ProductGroup = $"{productGroup.Key}",
                                    WinLossYesterday = productGroup.Sum(g => g.WinLossYesterday),
                                    WinLossMTD = productGroup.Sum(s => s.WinLossMTD),
                                    OrderByProduct = productGroup.Min(g => g.OrderByProduct) - 1,
                                });

                                productGroup.ToList().ForEach(product =>
                                {
                                    resultSubTotal.Add(new DailyDHWLModel
                                    {
                                        ProductShortName = product.ProductShortName,
                                        WinLossYesterday = product.WinLossYesterday,
                                        WinLossMTD = product.WinLossMTD,
                                        ProductGroup = product.ProductGroup,
                                        OrderByProduct = product.OrderByProduct,
                                    });
                                });
                            });
        }

        private static void CalcYesterdayAndMTDByProduct(DataTable dailyRawData, List<DailyDHWLModel> result, int yesterdayColumnIndex)
        {
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
        }

        private void CalcTotal(List<DailyDHWLModel> result, List<DailyDHWLModel> resultSubTotal)
        {
            var total = new DailyDHWLModel
            {
                ProductShortName = "Total",
                ProductGroup = "Total",
                OrderByProduct = 1000,
                WinLossYesterday = result.Sum(s => s.WinLossYesterday),
                WinLossMTD = result.Sum(s => s.WinLossMTD)
            };

            resultSubTotal.Add(total);
        }
    }
}