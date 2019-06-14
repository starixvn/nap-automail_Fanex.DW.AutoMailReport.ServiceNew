using System;

namespace Fanex.ESportModelingData.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsManual { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string FilePath { get; set; }

        public string Password { get; set; }

        public string DownloadLink { get; set; }
    }
}