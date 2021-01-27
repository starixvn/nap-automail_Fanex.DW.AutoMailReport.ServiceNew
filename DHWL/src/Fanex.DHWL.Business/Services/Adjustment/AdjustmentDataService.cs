using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.Adjustment
{
    public interface AdjustmentDataService
    {
        List<AdjustmentModel> CalcData(DataTable monthlyRawData);
    }

    public class AdjustmentDataServiceImp : AdjustmentDataService
    {
        public List<AdjustmentModel> CalcData(DataTable monthlyRawData)
        {
            var result = new List<AdjustmentModel>();
            var discount = CalcDiscountForLicensees(monthlyRawData);
            var rebate = CalcRebateForLicensees(monthlyRawData);
            var licenseeFee = CalcLicenseeFeeAdjustment(monthlyRawData);
            var royaltyFees = CalcRoyaltyFeesAdjustmentTo3rdParties(monthlyRawData);

            result.Add(discount);
            result.Add(rebate);
            result.Add(licenseeFee);
            result.Add(royaltyFees);

            result.Add(new AdjustmentModel
            {
                Id = "Total",
                Amount = result.Sum(s => s.Amount)
            });

            return result;
        }

        private AdjustmentModel CalcDiscountForLicensees(DataTable monthlyRawData)
        {
            var data = monthlyRawData
                .AsEnumerable()
                .Where(s => Converter.To<bool>(s[ColumnName.IsDeposit]) && s[ColumnName.SubFigure].Equals(RowFigureName.Discount));
            var monthly = data.Sum(s => Converter.To<decimal>(s[ColumnName.Monthly]));
            var mtd = data.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]));
            var amount = monthly - mtd;

            return new AdjustmentModel
            {
                Id = "1",
                Amount = amount,
                Description = "Discount for Licensees"
            };
        }

        private AdjustmentModel CalcRebateForLicensees(DataTable monthlyRawData)
        {
            var data = monthlyRawData
                .AsEnumerable()
                .Where(s => Converter.To<bool>(s[ColumnName.IsDeposit]) && s[ColumnName.SubFigure].Equals(RowFigureName.Rebate));
            var monthly = data.Sum(s => Converter.To<decimal>(s[ColumnName.Monthly]));
            var mtd = data.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]));
            var amount = monthly - mtd;

            return new AdjustmentModel
            {
                Id = "2",
                Amount = amount,
                Description = "Rebate for Licensees"
            };
        }

        private AdjustmentModel CalcLicenseeFeeAdjustment(DataTable monthlyRawData)
        {
            var data = monthlyRawData
                .AsEnumerable()
                .Where(s => Converter.To<bool>(s[ColumnName.IsDeposit]) &&
                (s[ColumnName.Figure].Equals(RowFigureName.LicenseeHouseIncome) || s[ColumnName.Figure].Equals(RowFigureName.LicensingMinimumFee)));
            var monthly = data.Sum(s => Converter.To<decimal>(s[ColumnName.Monthly]));
            var mtd = data.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]));
            var amount = monthly - mtd;

            return new AdjustmentModel
            {
                Id = "3",
                Amount = amount,
                Description = "Licensee Fee Adjustment"
            };
        }

        private AdjustmentModel CalcRoyaltyFeesAdjustmentTo3rdParties(DataTable monthlyRawData)
        {
            var data = monthlyRawData
                .AsEnumerable()
                .Where(s => s[ColumnName.Figure].Equals(RowFigureName.PaytoMuayThai)
                || s[ColumnName.Figure].Equals(RowFigureName.PaytoMetricGaming)
                || s[ColumnName.Figure].Equals(RowFigureName.PaytoBetGenius)
                || s[ColumnName.Figure].Equals(RowFigureName.CurrencyPlay)
                || s[ColumnName.Figure].Equals(RowFigureName.PaytoSupplier));
            var monthly = data.Sum(s => Converter.To<decimal>(s[ColumnName.Monthly]));
            var mtd = data.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]));
            var amount = monthly - mtd;

            return new AdjustmentModel
            {
                Id = "4",
                Amount = amount,
                Description = "Royalty Fees Adjustment to 3rd Parties"
            };
        }
    }
}