using System;

namespace Fanex.AbnormalBets.Business.Models.DailyLicensee
{
    public class SummaryBetModel
    {
        public int SportType { get; set; }

        public int LeagueId { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public DateTime KickOffTime { get; set; }

        public bool LiveIndicator { get; set; }

        public int Bettype { get; set; }

        public string Betteam { get; set; }

        public string Reason { get; set; }

        public int? FinalHomeScore { get; set; }

        public int? FinalAwayScore { get; set; }

        public int NumberOfLicensees { get; set; }

        public int NumberOfTickets { get; set; }
    }
}