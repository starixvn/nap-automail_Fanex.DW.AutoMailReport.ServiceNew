namespace Fanex.MarginChart.Business.Models
{
    public class DailyDHWLModel
    {
        public string ProductShortName { get; set; }

        public decimal WinLossYesterday { get; set; }

        public decimal WinLossMTD { get; set; }

        public string BreakColumn { get; }

        public string ProductGroup { get; set; }

        public int OrderByProduct { get; set; }
    }
}