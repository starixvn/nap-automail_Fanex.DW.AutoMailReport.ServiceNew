namespace NegativeBalance.Business.Models
{
    public class Site
    {
        public int SiteId { get; set; }

        public string LicenseeName { get; set; }

        public string ToList { get; set; }

        public string CcList { get; set; }

        public bool IsAvailable { get; set; }
    }
}