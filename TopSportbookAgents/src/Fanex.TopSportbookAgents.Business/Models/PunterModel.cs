using Fanex.TopSportbookAgents.Business.Models;

namespace Fanex.TopSportbookAgents.Business.Models
{
    public class PunterModel
    {
        public int Index { get; set; }

        public string Agent { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public decimal Winloss1 { get; set; }

        public decimal Turnover1 { get; set; }

        public decimal Margin1 { get; set; }

        public decimal Winloss2 { get; set; }

        public decimal Turnover2 { get; set; }

        public decimal Margin2 { get; set; }

        public decimal Winloss3 { get; set; }

        public decimal Turnover3 { get; set; }

        public decimal Margin3 { get; set; }
    }
}