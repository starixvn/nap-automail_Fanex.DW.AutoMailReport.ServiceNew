using System;
using Epplus.Mapper.Annotations;

namespace BetradarVirtualSports.Business.Models
{
    public class GrossWinningDaily
    {
        [Cell("A2")]
        public DateTime Date { get; set; }

        [Cell("B2")]
        public string Business { get; set; }

        [Cell("C2")]
        public string Currency { get; set; }

        [Cell("D2")]
        public int BetCount { get; set; }

        [Cell("E2")]
        public decimal TotalBet { get; set; }

        [Cell("F2")]
        public decimal TotalPayout { get; set; }

        [Cell("G2")]
        public decimal TotalWinLoss { get; set; }

        [Cell("H2")]
        public decimal Margin { get; set; }
    }
}