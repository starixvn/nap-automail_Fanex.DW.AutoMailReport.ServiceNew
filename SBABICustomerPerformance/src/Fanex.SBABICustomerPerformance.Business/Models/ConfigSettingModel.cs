using System;

namespace Fanex.SBABICustomerPerformance.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }

        public ReportType ReportType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}