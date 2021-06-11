using System.Collections.Generic;

namespace Fanex.AutoSyncFiles.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string CronSchedule { get; set; } = "0 0 0/1 * * ?";

        public FTPConfig FTPConfig { get; set; }

        public List<PathModel> Paths { get; set; }
    }
}