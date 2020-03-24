using System;

namespace Fanex.AbnormalBets.Business.Models.Licensee
{
    public class ShortTicketDetailModel
    {
        public string Username { get; set; }

        public string Username2 { get; set; }

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
    }
}