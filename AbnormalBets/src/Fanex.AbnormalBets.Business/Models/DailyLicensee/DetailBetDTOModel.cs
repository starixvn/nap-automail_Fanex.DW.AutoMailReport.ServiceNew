using System;

namespace Fanex.AbnormalBets.Business.Models.DailyLicensee
{
    public class DetailBetDTOModel
    {
        public string Sport { get; set; }

        public string League { get; set; }

        public string Match { get; set; }

        public string Home { get; set; }

        public DateTime KickOffTime { get; set; }

        public string Live { get; set; }

        public int NumberOfLicensees { get; set; }

        public int NumberOfTickets { get; set; }

        public string Bettype { get; set; }

        public string Betteam { get; set; }

        public string Reason { get; set; }

        public string FinalScore { get; set; }

        public string Username { get; set; }

        public decimal Odds { get; set; }

        public string CurrencyName { get; set; }

        public decimal Stake { get; set; }

        public decimal Hdp { get; set; }

        public int? LiveHomeScore { get; set; }

        public int? LiveAwayScore { get; set; }

        public DateTime TransDate { get; set; }

        public long TransId { get; set; }
    }
}