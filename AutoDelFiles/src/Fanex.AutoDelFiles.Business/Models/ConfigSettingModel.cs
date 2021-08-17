using System.Collections.Generic;

namespace Fanex.AutoDelFiles.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string CronSchedule { get; set; } = "0 0 0/1 * * ?";

        public List<PathModel> Paths { get; set; }
    }
}