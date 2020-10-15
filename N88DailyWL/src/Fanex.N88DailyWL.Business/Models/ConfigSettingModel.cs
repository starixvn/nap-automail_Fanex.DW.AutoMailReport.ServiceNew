using System;
using System.Collections.Generic;

namespace Fanex.N88DailyWL.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public string Path { get; set; }

        public string TargetName { get; set; }

        public List<LicenseeNameGroup> LicenseeNameGroups { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public ImageProperty Currency { get; set; }

        public ImageProperty Product { get; set; }

        public bool IsSentDirectly { get; set; }

        public string InternalPath { get; set; }
    }
}