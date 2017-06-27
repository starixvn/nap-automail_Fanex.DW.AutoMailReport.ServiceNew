using System;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;

namespace NegativeBalance.DatabaseTests
{
    public static class FanexDataRegisterService
    {
        public static void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($"~/App_Data/Local", enableWatching: true)
                .Run();
        }
    }
}