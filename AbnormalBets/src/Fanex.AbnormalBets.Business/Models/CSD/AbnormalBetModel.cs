using System;

namespace Fanex.AbnormalBets.Business.Models.CSD
{
    public class AbnormalBetDailyModel
    {
        public DateTime Date { get; set; }

        public int SportType { get; set; }

        public int Leagueid { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public DateTime GlobalShowTime { get; set; }

        public string Bettypes { get; set; }

        public int? HtHomeScore { get; set; }

        public int? HtAwayScore { get; set; }

        public int? FinalHomeScore { get; set; }

        public int? FinalAwayScore { get; set; }

        public string UserName { get; set; }

        public int Live { get; set; }

        public int NonLive { get; set; }

        public int TotalOfTickets { get; set; }

        public string Reason { get; set; }

        public string Sites { get; set; }

        public string AffectedSite { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}