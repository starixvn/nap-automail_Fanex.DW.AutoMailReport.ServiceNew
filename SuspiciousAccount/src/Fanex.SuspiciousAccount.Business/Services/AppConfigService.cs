using Fanex.SuspiciousAccount.Business.Models;

namespace Fanex.SuspiciousAccount.Business.Services
{
    public static class AppConfigService
    {
        public static ConfigSettingModel Config { get; set; } = new ConfigSettingModel();
    }
}