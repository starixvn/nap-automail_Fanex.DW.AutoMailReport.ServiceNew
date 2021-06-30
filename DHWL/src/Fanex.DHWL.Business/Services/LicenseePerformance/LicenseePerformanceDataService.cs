using Fanex.Common;
using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Models.LicenseeDHWLLetstalk;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.LicenseePerformance
{
    public interface LicenseePerformanceDataService
    {
        List<LicenseePerformanceModel> CalTotalByProduct(DataTable licPerfRawData);
    }

    public class LicenseePerformanceDataServiceImp : LicenseePerformanceDataService
    {
        public List<LicenseePerformanceModel> CalTotalByProduct(DataTable licPerfRawData)
        {
            var result = new List<LicenseePerformanceModel>();
            var yesterdayColumnIndex = licPerfRawData.Columns.Count - 1;

            licPerfRawData.AsEnumerable()
                .Where(s => Converter.To<bool>(s[ColumnName.IsDeposit]))
                .GroupBy(g => new
                {
                    ProductShortName = g[ColumnName.ProductShortName].ToString(),
                    ProductGroup = g[ColumnName.ProductGroup].ToString(),
                    OrderByProduct = Converter.To<int>(g[ColumnName.OrderByProduct])
                })
                .ToList()
                .ForEach(product =>
                {
                    var row = new LicenseePerformanceModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        OrderByProduct = product.Key.OrderByProduct,
                        ProductGroup = product.Key.ProductGroup,

                        TO = product.Where(s => s[ColumnName.SubFigure].Equals(RowFigureName.LicenseeTurnover)).Sum(s => Converter.To<decimal>(s[yesterdayColumnIndex])),
                        WL = product.Where(s => s[ColumnName.SubFigure].Equals(RowFigureName.LicenseeWinloss)).Sum(s => Converter.To<decimal>(s[yesterdayColumnIndex])),
                        TOMTD = product.Where(s => s[ColumnName.SubFigure].Equals(RowFigureName.LicenseeTurnover)).Sum(s => Converter.To<decimal>(s[ColumnName.MTD])),
                        WLMTD = product.Where(s => s[ColumnName.SubFigure].Equals(RowFigureName.LicenseeWinloss)).Sum(s => Converter.To<decimal>(s[ColumnName.MTD])),

                        LicenseeFee = product.Where(s => s[ColumnName.SubFigure].Equals(RowFigureName.LicenseeHouseIncome)).Sum(s => Converter.To<decimal>(s[ColumnName.Monthly])),
                    };

                    result.Add(row);
                });

            var royaltyFees = licPerfRawData.AsEnumerable()
                .Where(s => Converter.To<bool>(s[ColumnName.IsDeposit]) && RowFigureName.RoyaltyFees.Contains(s[ColumnName.SubFigure]))
                .Sum(s => Converter.To<decimal>(s[ColumnName.Monthly]));

            CalcTotal(result, royaltyFees);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private void CalcTotal(List<LicenseePerformanceModel> result, decimal royaltyFees)
        {
            var total = new LicenseePerformanceModel
            {
                ProductGroup = "Total",
                ProductShortName = "Total",
                OrderByProduct = 1000,
                TO = result.Sum(s => s.TO),
                WL = result.Sum(s => s.WL),
                TOMTD = result.Sum(s => s.TOMTD),
                WLMTD = result.Sum(s => s.WLMTD),
                LicenseeFee = result.Sum(s => s.LicenseeFee),
            };

            var totalRoyaltyFee = new LicenseePerformanceModel
            {
                ProductGroup = "Total",
                ProductShortName = "Royalty Fees To 3rd parties",
                OrderByProduct = 1001,
                LicenseeFee = royaltyFees,
            };

            var netTotal = new LicenseePerformanceModel
            {
                ProductGroup = "Total",
                ProductShortName = "NET TOTAL",
                OrderByProduct = 1002,
                TO = result.Sum(s => s.TO),
                WL = result.Sum(s => s.WL),
                TOMTD = result.Sum(s => s.TOMTD),
                WLMTD = result.Sum(s => s.WLMTD),
                LicenseeFee = result.Sum(s => s.LicenseeFee) + royaltyFees,
            };

            result.Add(total);
            result.Add(totalRoyaltyFee);
            result.Add(netTotal);
        }
    }
}