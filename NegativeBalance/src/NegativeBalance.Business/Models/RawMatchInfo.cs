using System;

namespace NegativeBalance.Business.Models
{
    public class RawMatchInfo
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        public string MatchCode { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int SportId { get; set; }
    }
}