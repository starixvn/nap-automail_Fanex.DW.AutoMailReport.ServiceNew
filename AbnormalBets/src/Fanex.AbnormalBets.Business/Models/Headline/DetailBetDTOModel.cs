using System;

namespace Fanex.AbnormalBets.Business.Models.Headline
{
    public class DetailBetDTOModel
    {
        public string Username { get; set; }

        public decimal Odds { get; set; }

        public string CurrencyName { get; set; }

        public decimal Stake { get; set; }

        public decimal Hdp { get; set; }

        public int? LiveHomeScore { get; set; }

        public int? LiveAwayScore { get; set; }

        public DateTime TransDate { get; set; }

        public long TransId { get; set; }

        public string Ip { get; set; }

        public string Location { get; set; }

        public string Sport { get; set; }

        public string League { get; set; }

        public string Match { get; set; }

        public DateTime KickOffTime { get; set; }

        public string Live { get; set; }

        public string Bettype { get; set; }

        public string Betteam { get; set; }

        public string Reason { get; set; }

        public string FinalScore { get; set; }

        public string Super { get; set; }

        public string Master { get; set; }

        public string Agent { get; set; }
    }
}