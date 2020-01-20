using Epplus.Mapper.Annotations;
using System;

namespace Fanex.CN88DailyPerformance.Service.Model
{
    public class DailyPerformanceModel
    {
        [Cell("A2")]
        public string Sport { get; set; }

        [Cell("B2")]
        public DateTime Date { get; set; }

        [Cell("C2")]
        public int ClientId { get; set; }

        [Cell("D2")]
        public string UserName { get; set; }

        [Cell("E2")]
        public string League { get; set; }

        [Cell("F2")]
        public string Match { get; set; }

        [Cell("G2")]
        public string BetType { get; set; }

        [Cell("H2")]
        public int BetCount { get; set; }

        [Cell("I2")]
        public string Live { get; set; }

        [Cell("J2")]
        public string Site { get; set; }

        [Cell("K2")]
        public decimal RealCompanyTurnover { get; set; }

        [Cell("L2")]
        public decimal RealCompanyWinloss { get; set; }

        [Cell("M2")]
        public decimal TurnoverMember { get; set; }

        [Cell("N2")]
        public decimal WinlossMember { get; set; }
    }
}
