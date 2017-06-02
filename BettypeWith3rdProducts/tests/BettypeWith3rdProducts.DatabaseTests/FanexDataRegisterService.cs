using System;
using System.Configuration;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;

namespace BettypeWith3rdProducts.DatabaseTests
{
    internal static class FanexDataRegisterService
    {
        internal static void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var envrironment = ConfigurationManager.AppSettings["Environment"];
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(resourcePath: $"~/App_Data/{envrironment}", ignoreRedundantParameters: false, enableWatching: true)
                .Run();
        }
    }
}