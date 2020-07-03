namespace Fanex.TopLicensee.Service.Models
{
    public class TopLicenseeMarginModel
    {
        public int Order { get; set; }

        public string Licensee { get; set; }

        public decimal MarginAllSport { get; set; }

        public decimal MarginSoccer { get; set; }

        public decimal MarginBasketball { get; set; }

        public decimal MarginEsport { get; set; }

        public decimal MarginTennis { get; set; }

        public decimal MarginParlay { get; set; }

        public decimal MarginSabaSoccer { get; set; }

        public decimal MarginSabaBasketball { get; set; }

        public decimal MarginOthersport { get; set; }
    }
}