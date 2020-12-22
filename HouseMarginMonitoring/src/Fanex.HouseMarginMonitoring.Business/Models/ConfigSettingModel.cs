using System;
using System.Collections.Generic;

namespace Fanex.HouseMarginMonitoring.Business.Models
{
    public class ConfigSettingModel
    {
        public bool IsCheckEnoughData { get; set; }

        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string Path { get; set; }

        public string InternalPath { get; set; }

        public string TargetName { get; set; }

        public string Password { get; set; }

        public int MailId { get; set; }

        public string CronSchedule { get; set; } = "0 0 0/1 * * ?";

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal? WLThreshold { get; set; }

        public decimal? MarginThreshold { get; set; }

        public Dictionary<string, ImageProperty> ImageSettings { get; set; } = new Dictionary<string, ImageProperty>();

        public HouseMarginSettings HouseMarginMonitoringSettings { get; set; }
    }
}