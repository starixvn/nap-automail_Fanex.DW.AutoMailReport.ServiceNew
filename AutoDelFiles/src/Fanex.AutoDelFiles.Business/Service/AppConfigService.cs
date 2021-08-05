using Fanex.AutoDelFiles.Business.Models;

namespace Fanex.AutoDelFiles.Business.Service
{
    public static class AppConfigService
    {
        public static ConfigSettingModel AppConfig { get; set; }

        public static bool IsProcessing { get; set; }
    }
}