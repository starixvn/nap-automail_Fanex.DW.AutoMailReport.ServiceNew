namespace NegativeBalance.Business.Models
{
    public class AppConfig
    {
        public int IntervalInSecond { get; set; }

        public bool IsTest { get; set; }

        public bool IsSendMailFromConfig { get; set; }

        public string MailTo { get; set; }

        public string MailCc { get; set; }

        public string MailBcc { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }
    }
}