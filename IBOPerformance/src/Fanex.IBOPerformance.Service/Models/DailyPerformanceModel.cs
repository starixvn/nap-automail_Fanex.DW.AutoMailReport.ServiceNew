namespace Fanex.IBOPerformance.Service.Models
{
    public class DailyPerformanceModel
    {
        public int Counted { get; set; }

        public string UserName { get; set; }

        public decimal YesterdayTurnover { get; set; }

        public decimal BetCount { get; set; }

        public decimal Winloss { get; set; }

        public decimal Turnover { get; set; }

        public decimal Margin
        {
            get
            {
                return Turnover == 0
                    ? 0
                    : Winloss / Turnover;
            }
        }
    }
}