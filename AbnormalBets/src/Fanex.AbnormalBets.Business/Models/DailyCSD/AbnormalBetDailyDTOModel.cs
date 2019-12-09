using System;

namespace Fanex.AbnormalBets.Business.Models.DailyCSD
{
    public class AbnormalBetDailyDTOModel
    {
        public DateTime Date { get; set; }

        public string Sport { get; set; }

        public string League { get; set; }

        public string Match { get; set; }

        public DateTime KickOffTime { get; set; }

        public int Live { get; set; }

        public int NonLive { get; set; }

        public int TotalOfTickets { get; set; }

        public string Sites { get; set; }

        public string AffectedSite { get; set; }

        public string Bettypes { get; set; }

        public string Reason { get; set; }

        public string HTScore { get; set; }

        public string FinalScore { get; set; }

        public string Username { get; set; }
    }
}