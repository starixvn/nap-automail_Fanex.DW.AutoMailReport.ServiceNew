namespace Fanex.SoccerNonLive.Business.Models
{
    public class SoccerModel
    {
        public string LeagueName { get; set; }

        public string MatchEvent { get; set; }

        public string BettypeName { get; set; }

        public int BetCount { get; set; }

        public decimal ReallIncome { get; set; }

        public decimal VirtualIncome { get; set; }

        public decimal RealTurnover { get; set; }

        public decimal VirtualTurnover { get; set; }
    }
}