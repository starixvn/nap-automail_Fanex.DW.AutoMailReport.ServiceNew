using System;

namespace Fanex.ServiceMonitoring.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public int IntervalInSecondServiceMonitor { get; set; }

        public int IntervalInSecondMailMonitor { get; set; }

        public string MailTo { get; set; }

        public string DataType { get; set; }

        public string TargetName { get; set; }

        public string LetstalkGroup { get; set; }

        public string Environment { get; set; }

        public string AllowIP { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}