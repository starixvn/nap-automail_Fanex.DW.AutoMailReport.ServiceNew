using System.Collections.Generic;

namespace Fanex.Letstalk.OneTime.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string Path { get; set; }

        public List<FolderRegisterModel> Folders { get; set; }
    }
}