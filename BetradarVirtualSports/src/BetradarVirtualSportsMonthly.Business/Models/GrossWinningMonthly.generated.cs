using System;
using Epplus.Mapper.Annotations;

namespace BetradarVirtualSportsMonthly.Business.Models
{
    public class GrossWinningMonthly
    {
        [Cell("A4")]
        public DateTime Date { get; set; }

        [Cell("B4")]
        public string GameType { get; set; }

        [Cell("C4")]
        public string Business { get; set; }

        [Cell("D4")]
        public string Currency { get; set; }        

        [Cell("E4")]
        public string DeviceGroup { get; set; }

        [Cell("F4")]
        public int BetCount { get; set; }

        [Cell("G4")]
        public decimal TotalBet { get; set; }

        [Cell("H4")]
        public decimal TotalPayOut { get; set; }

        [Cell("I4")]
        public decimal TotalWinLoss { get; set; }

        [Cell("J4")]
        public decimal Margin { get; set; }
    }
}