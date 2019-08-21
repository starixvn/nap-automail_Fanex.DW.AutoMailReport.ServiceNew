using System;

namespace Fanex.ScoreBox.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsManual { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public bool IsDaily { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FilePath { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }
    }
}