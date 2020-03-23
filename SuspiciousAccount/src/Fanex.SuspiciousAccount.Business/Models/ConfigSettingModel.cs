namespace Fanex.SuspiciousAccount.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string FPSApi { get; set; }

        public string TargetName { get; set; }

        public string GroupName { get; set; }

        public int IntervalInSecond { get; set; }

        public string ServiceName { get; set; }
    }
}