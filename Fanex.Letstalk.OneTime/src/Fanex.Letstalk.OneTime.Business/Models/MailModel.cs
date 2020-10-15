namespace Fanex.Letstalk.OneTime.Business.Models
{
    public class MailModel
    {
        public int MailId { get; set; }

        public string MailName { get; set; }

        public bool IsAllowResend { get; set; }
    }
}