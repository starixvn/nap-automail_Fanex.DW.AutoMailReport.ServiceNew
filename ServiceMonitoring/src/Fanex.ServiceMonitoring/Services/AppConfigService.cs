using System.Collections.Generic;
using Fanex.ServiceMonitoring.Models;

namespace Fanex.ServiceMonitoring.Services
{
    public static class AppConfigService
    {
        public static ConfigSettingModel AppConfig
        {
            get; set;
        }

        public static List<string> MailNoNeedToMonitor
        {
            get; set;
        }
    }
}