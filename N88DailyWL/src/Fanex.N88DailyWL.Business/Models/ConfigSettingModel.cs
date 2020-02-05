using System;

namespace Fanex.N88DailyWL.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public string Path { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public ImageProperty N88 { get; set; }

        public ImageProperty Deposit { get; set; }
    }
}