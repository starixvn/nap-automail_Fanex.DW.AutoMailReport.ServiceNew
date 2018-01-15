using System;
using System.Configuration;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;

namespace Fanex.UpdateExchangeRate.Executing
{
    internal static class FanexDataRegisterService
    {
        internal static void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var storedProcedurePath = ConfigurationManager.AppSettings["StoredProcedurePath"];
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($@"~/{storedProcedurePath}", enableWatching: true)
                .Run();
        }
    }
}