namespace Fanex.ServiceMonitoring.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public int IntervalInSecond { get; set; }

        public string MailTo { get; set; }
    }
}