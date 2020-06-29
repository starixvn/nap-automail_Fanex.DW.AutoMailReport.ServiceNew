using Fanex.ESportModelingData.Business.Models;
using System;

namespace Fanex.ESportModelingData.Business.Services
{
    public static class AppConfigService
    {
        public static ConfigSettingModel AppConfig
        {
            get; set;
        }

        public static DateTime DateStart { get; set; }
    }
}