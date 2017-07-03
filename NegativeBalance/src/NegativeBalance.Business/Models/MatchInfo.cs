using System;

namespace NegativeBalance.Business.Models
{
    public class MatchInfo
    {
        public string LeagueName { get; set; } = string.Empty;

        public string LeagueNameCS { get; set; } = string.Empty;

        public string HomeName { get; set; } = string.Empty;

        public string HomeNameCS { get; set; } = string.Empty;

        public string AwayName { get; set; } = string.Empty;

        public string AwayNameCS { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string MatchCode { get; set; } = string.Empty;

        public int MatchId { get; set; } 

        public int GroupId { get; set; }

        public string SportName { get; set; } = string.Empty;

        public string SportNameCS { get; set; } = string.Empty;
    }
}