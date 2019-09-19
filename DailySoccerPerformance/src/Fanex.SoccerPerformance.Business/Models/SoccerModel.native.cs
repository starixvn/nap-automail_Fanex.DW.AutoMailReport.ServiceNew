namespace Fanex.SoccerPerformance.Business.Models
{
    public class SoccerModel
    {
        public string Bettype { get; set; }

        public decimal RealTurnover { get; set; }

        public decimal RealWinLoss { get; set; }

        public decimal RealMargin { get; set; }

        public decimal VirtualTurnover { get; set; }

        public  decimal VirtualWinLoss { get; set; }

        public decimal VirtualMargin { get; set; }
    }
}
