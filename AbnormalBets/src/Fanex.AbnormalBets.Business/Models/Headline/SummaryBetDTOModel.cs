using System;

namespace Fanex.AbnormalBets.Business.Models.Headline
{
    public class SummaryBetDTOModel
    {
        public string Sport { get; set; }

        public string League { get; set; }

        public string Match { get; set; }

        public DateTime KickOffTime { get; set; }

        public string Live { get; set; }

        public int NumberOfLicensees { get; set; }

        public int NumberOfTickets { get; set; }

        public string Bettype { get; set; }

        public string Betteam { get; set; }

        public string Reason { get; set; }

        public string FinalScore { get; set; }

        public string Super { get; set; }

        public string Master { get; set; }

        public string Agent { get; set; }
    }
}