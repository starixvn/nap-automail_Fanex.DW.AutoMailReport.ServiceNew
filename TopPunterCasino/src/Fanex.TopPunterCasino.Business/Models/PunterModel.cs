using Fanex.TopPunterCasino.Business.Models;

namespace Fanex.TopPunterCasino.Business.Models
{
    public class PunterModel
    {
        public int Index { get; set; }

        public string Member { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public decimal Turnover { get; set; }

        public decimal Winloss { get; set; }

        public decimal Margin { get; set; }

        public decimal TurnoverMTD { get; set; }

        public decimal WinlossMTD { get; set; }

        public decimal MarginMTD { get; set; }

        public CasinoType CasinoType { get; set; }
    }
}