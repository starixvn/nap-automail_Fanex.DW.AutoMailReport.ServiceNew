using System;

namespace Fanex.BetBuilder.Business.Models
{
    public class DailyRawDataModel
    {
        public string Sport { get; set; }

        public int LeagueId { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public DateTime? GlobalShowTime { get; set; }

        public string MarketId { get; set; }

        public DateTime? DateTimePlaced { get; set; }

        public string BetDescription { get; set; }

        public int? NoOfLegsPerBet { get; set; }

        public string BetStatus { get; set; }

        public string DeviceGroup { get; set; }

        public decimal? BetPrice { get; set; }

        public string Code { get; set; }

        public decimal Stake { get; set; }

        public string Result { get; set; }

        public decimal Payout { get; set; }

        public decimal Commission { get; set; }

        public decimal GrossRevenue { get; set; }
    }
}