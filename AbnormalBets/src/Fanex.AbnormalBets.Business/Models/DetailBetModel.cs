using System;

namespace Fanex.AbnormalBets.Business.Models
{
    public class DetailBetModel
    {
        public long TransId { get; set; }

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

        public string Site { get; set; }

        public string Username { get; set; }

        public decimal Odds { get; set; }

        public string CurrencyName { get; set; }

        public decimal Stake { get; set; }

        public decimal Hdp { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public DateTime TransDate { get; set; }
    }
}