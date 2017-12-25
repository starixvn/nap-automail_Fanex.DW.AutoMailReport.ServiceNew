using System;
using Epplus.Mapper.Annotations;

namespace BetradarVirtualSportsDaily.Business.Models
{
    public class GrossWinningDaily
    {
        [Cell("A4")]
        public DateTime Date { get; set; }

        [Cell("B4")]
        public string Business { get; set; }

        [Cell("C4")]
        public string Currency { get; set; }

        [Cell("D4")]
        public string DeviceGroup { get; set; }

        [Cell("E4")]
        public int BetCount { get; set; }

        [Cell("F4")]
        public decimal TotalBet { get; set; }

        [Cell("G4")]
        public decimal TotalPayout { get; set; }

        [Cell("H4")]
        public decimal TotalWinLoss { get; set; }

        [Cell("I4")]
        public decimal Margin { get; set; }
    }
}