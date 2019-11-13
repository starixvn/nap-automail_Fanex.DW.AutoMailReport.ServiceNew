using System;

namespace Fanex.ESportDataMining.Business.Models
{
    public class MatchModel
    {
        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public DateTime MatchStartTime { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }
    }
}