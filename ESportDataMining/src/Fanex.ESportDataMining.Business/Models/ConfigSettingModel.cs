using System;

namespace Fanex.ESportDataMining.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FilePath { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }
    }
}