using System;
using CsvHelper.Configuration.Attributes;

namespace Fanex.BetBuilder.Business.Models
{
    public class DailyDataModel
    {
        public string Sport { get; set; }

        [Name("Competition Name")]
        public string LeagueName { get; set; }

        [Name("Fixture Name")]
        public string FixtureName { get; set; }

        [Name("Fixture Start Date/Time")]
        public DateTime? GlobalShowTime { get; set; }

        [Name("Market Identifier (Unique to each combination placed)")]
        public string MarketId { get; set; }

        [Name("Date/Time placed")]
        public DateTime? DateTimePlaced { get; set; }

        [Name("Bet Description")]
        public string BetDescription { get; set; }

        [Name("No. of legs per bet")]
        public int? NoOfLegsPerBet { get; set; }

        [Name("Bet Status (Settled/Open)")]
        public string BetStatus { get; set; }

        [Name("Bet Platform (Desktop/Mobile)")]
        public string DeviceGroup { get; set; }

        [Name("Bet Price (Odds)")]
        public decimal? BetPrice { get; set; }

        [Name("Currency")]
        public string Code { get; set; }
        
        public decimal Stake { get; set; }

        [Name("Bet Status/Result")]
        public string Result { get; set; }

        public decimal Payout { get; set; }

        public decimal Commission { get; set; }

        [Name("Gross Revenue")]
        public decimal GrossRevenue { get; set; }
    }
}