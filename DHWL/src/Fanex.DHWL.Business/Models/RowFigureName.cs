using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models
{
    public class RowFigureName
    {
        public const string HouseTurnOver = "House TurnOver";
        public const string ContributionAmount = "Contribution Amount";
        public const string CreditHouseIncome = "Credit House Income";
        public const string ExtraPT = "Extra PT";
        public const string JackpotPrize = "Jackpot Prize";
        public const string MegaPrize = "Mega Prize";
        public const string PaytoBetGenius = "Pay to Bet Genius";
        public const string PaytoSupplier = "Pay to Supplier";
        public const string PaytoMetricGaming = "Pay to Metric Gaming";
        public const string PaytoMuayThai = "Pay to Muay Thai";

        public const string RebateDiscount = "Rebate & Discount";
        public const string CurrencyPlay = "Currency Play";
        public const string LicensingMinimumFee = "Licensing Minimum Fee";

        public static List<string> IgnoreFigureRows = new List<string>
        {
            PaytoSupplier,
            RebateDiscount,
            CurrencyPlay,
            LicensingMinimumFee,
            PaytoBetGenius,
            PaytoMetricGaming,
            PaytoMuayThai,
            PaytoSupplier
        };
    }
}