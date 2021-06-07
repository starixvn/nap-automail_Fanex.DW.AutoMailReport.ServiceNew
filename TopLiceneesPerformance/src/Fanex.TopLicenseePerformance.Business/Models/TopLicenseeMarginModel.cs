namespace Fanex.TopLicensee.Service.Models
{
    public class TopLicenseeMarginModel
    {
        public int Order { get; set; }

        public string LicenseeGroup { get; set; }

        public string Licensee { get; set; }

        public decimal MarginAllSport_Group { get; set; }

        public decimal MarginSoccer_Group { get; set; }

        public decimal MarginBasketball_Group { get; set; }

        public decimal MarginEsport_Group { get; set; }

        public decimal MarginTennis_Group { get; set; }

        public decimal MarginParlay_Group { get; set; }

        public decimal MarginSabaSoccer_Group { get; set; }

        public decimal MarginSabaSoccerPinGoal_Group { get; set; }

        public decimal MarginSabaBasketball_Group { get; set; }

        public decimal MarginOthersport_Group { get; set; }

        public string BreakColumn { get; set; } = string.Empty;

        public decimal MarginAllSport { get; set; }

        public decimal MarginSoccer { get; set; }

        public decimal MarginBasketball { get; set; }

        public decimal MarginEsport { get; set; }

        public decimal MarginTennis { get; set; }

        public decimal MarginParlay { get; set; }

        public decimal MarginSabaSoccer { get; set; }

        public decimal MarginSabaSoccerPinGoal { get; set; }

        public decimal MarginSabaBasketball { get; set; }

        public decimal MarginOthersport { get; set; }
    }
}