using System;

namespace Fanex.BetBuilder.Business.Models
{
    public class MonthlyDataModel
    {
        public string Sport { get; set; }

        public string LeagueName { get; set; }

        public string FixtureName { get; set; }

        public DateTime? GlobalShowTime { get; set; }

        public int? MarketId { get; set; }

        public DateTime? DateTimePlaced { get; set; }

        public DateTime? WinLostDate { get; set; }
        public long TransId { get; set; }

        public string BetDescription { get; set; }

        public int? NoOfLegsPerBet { get; set; }

        public string BetStatus { get; set; }

        public string DeviceGroup { get; set; }

        public decimal? BetPrice { get; set; }

        public string Code { get; set; }

        public decimal? StakeBefore { get; set; }

        public decimal? StakeAfter { get; set; }

        public string ResultBefore { get; set; }

        public string ResultAfter { get; set; }

        public decimal? PayoutBefore { get; set; }

        public decimal? PayoutAfter { get; set; }

        public decimal? CommissionBefore { get; set; }

        public decimal? CommissionAfter { get; set; }

        public decimal? GrossRevenueBefore { get; set; }

        public decimal? GrossRevenueAfter { get; set; }
    }
}