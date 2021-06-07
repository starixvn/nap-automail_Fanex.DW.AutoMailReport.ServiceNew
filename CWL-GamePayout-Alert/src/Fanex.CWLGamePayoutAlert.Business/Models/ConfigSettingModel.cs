using System;

namespace Fanex.CWLGamePayoutAlert.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public int MailId { get; set; }

        public int NoDataMailId { get; set; }

        public string CronSchedule { get; set; } = "0 0 0/1 * * ?";

        public bool IsTest { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromGMT8Time { get; set; }

        public int TimeInterval { get; set; } = 1;

        public decimal TLV { get; set; } = 400;

        public decimal MWinLV { get; set; } = 10000;
    }
}