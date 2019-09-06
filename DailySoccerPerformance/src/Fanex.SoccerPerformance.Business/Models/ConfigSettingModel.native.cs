using System;

namespace Fanex.SoccerPerformance.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string FilePath { get; set; }

        public bool IsManual { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public int FunctionIdNodata { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime Yesterday { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }
    }
}
