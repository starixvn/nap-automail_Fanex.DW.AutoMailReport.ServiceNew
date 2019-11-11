using System;

namespace Fanex.ESportDataMining.Business.Models
{
    public class FactCustEsportModel
    {
        public DateTime DateKey { get; set; }

        public int CurrencyId { get; set; }

        public int SiteId { get; set; }

        public int LiveStatusId { get; set; }

        public int BetType { get; set; }

        public int Tier { get; set; }

        public int? BettypeDetailX { get; set; }

        public int? BettypeDetailY { get; set; }

        public int LeagueId { get; set; }

        public int CustId { get; set; }

        public int TraderId { get; set; }

        public int MatchId { get; set; }

        public decimal VRSetting { get; set; }

        public decimal MemberWinloss { get; set; }

        public decimal MemberTurnover { get; set; }

        public decimal HouseRealIncome { get; set; }

        public decimal HouseRealTurnover { get; set; }

        public decimal Commission { get; set; }

        public int BetCount { get; set; }

        public decimal HouseVirtualIncome { get; set; }

        public decimal HouseVirtualTurnover { get; set; }

        public decimal VarianceIncome { get; set; }

        public decimal VarianceTurnover { get; set; }
    }
}