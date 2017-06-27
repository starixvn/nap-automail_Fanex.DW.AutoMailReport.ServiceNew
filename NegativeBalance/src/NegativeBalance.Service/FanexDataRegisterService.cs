using System;
using System.Configuration;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;

namespace NegativeBalance.Service
{
    public static class FanexDataRegisterService
    {
        public static void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var envrironment = ConfigurationManager.AppSettings["Environment"];
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($"~/App_Data/{envrironment}", enableWatching: true)
                .Run();
        }
    }
}