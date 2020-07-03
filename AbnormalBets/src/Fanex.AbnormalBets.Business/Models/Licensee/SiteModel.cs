namespace Fanex.AbnormalBets.Business.Models.Licensee
{
    public class SiteModel
    {
        public int MailId { get; set; }

        public string MailName { get; set; }

        public string SiteName { get; set; }

        public string Subject { get; set; }

        public bool IsHideAfterPTColumn { get; set; } = true;
    }
}