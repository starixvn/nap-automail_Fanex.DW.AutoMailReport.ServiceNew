namespace Fanex.MarginChart.Business.Models
{
    public class MonthlyDHWLModel
    {
        public string ProductShortName { get; set; }

        public decimal WinLossMTD { get; set; }

        public string BreakColumn { get; }

        public decimal WinLossMonthly { get; set; }

        public string ProductGroup { get; set; }

        public int OrderByProduct { get; set; }
    }
}