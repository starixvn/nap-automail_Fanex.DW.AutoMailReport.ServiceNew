using System.Collections.Generic;

namespace Fanex.AutoSyncFiles.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public int TimerPeriodInSecond { get; set; } = 300;

        public FTPConfig FTPConfig { get; set; }

        public List<PathModel> Paths { get; set; }
    }
}