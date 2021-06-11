using Fanex.AutoSyncFiles.Business.Models;

namespace Fanex.AutoSyncFiles.Business.Services
{
    public static class AppConfigService
    {
        public static ConfigSettingModel AppConfig { get; set; }

        public static bool IsProcessing { get; set; }
    }
}