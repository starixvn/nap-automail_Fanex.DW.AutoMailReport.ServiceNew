namespace Fanex.TopLicenseePerformance.Business.Models
{
    public class TopLeagueGroupSportLicenseePerfModel
    {
        public int Order { get; set; }

        public string LicGroup { get; set; }

        public string Licensee { get; set; }

        public decimal LicGroupTO_Yesterday { get; set; }

        public decimal LicGroupMargin_Yesterday { get; set; }

        public decimal LicGroupTO_MTD { get; set; }

        public decimal LicGroupMargin_MTD { get; set; }

        public string BreakColumn { get; set; } = string.Empty;

        public decimal LicTO_Yesterday { get; set; }

        public decimal LicMargin_Yesterday { get; set; }

        public decimal LicTO_MTD { get; set; }

        public decimal LicMargin_MTD { get; set; }
    }
}