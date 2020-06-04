using System;

namespace Fanex.ESportModelingData.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string FilePath { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }

        public string DownloadLink { get; set; }

        public Period Period { get; set; }

        public Sport Sport { get; set; }
    }
}