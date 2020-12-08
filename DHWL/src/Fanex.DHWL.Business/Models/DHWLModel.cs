namespace Fanex.DHWL.Business.Models
{
    public class DHWLModel
    {
        public string ProductShortName { get; set; }

        public decimal WinLossYesterday { get; set; }

        public decimal WinLossMTD { get; set; }

        public string BreakColumn { get; }

        public string ProductGroup { get; set; }

        public int OrderByProduct { get; set; }
    }
}