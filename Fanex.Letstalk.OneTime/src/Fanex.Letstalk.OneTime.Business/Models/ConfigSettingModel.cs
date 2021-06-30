using System;
using System.Collections.Generic;

namespace Fanex.Letstalk.OneTime.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string Path { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }

        public bool IsManual { get; set; }

        public DateTime Date { get; set; }

        public Dictionary<string, MailModel> MailSettings { get; set; }

        public List<FolderRegisterModel> Folders { get; set; }
    }
}