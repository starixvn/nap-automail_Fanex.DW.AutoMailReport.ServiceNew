using System;

namespace Fanex.CN88DailyWL.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string FilePath { get; set; }

        public bool IsManual { get; set; }

        public DateTime WinLostDate { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }
    }
}