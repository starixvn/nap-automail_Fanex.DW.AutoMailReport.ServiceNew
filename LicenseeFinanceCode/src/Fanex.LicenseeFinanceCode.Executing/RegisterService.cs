using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LicenseeFinanceCode.Business.Model;
using Fanex.LicenseeFinanceCode.Business.Service;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.LicenseeFinanceCode.Executing
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            LoadConfig();
            SentryRegister();
            ConfigureFanexData();
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DataService, DataServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<NotificationService, NotificationServiceImp>();
        }

        private static void SentryRegister()
        {
            var template = File.ReadAllText("Template/Settings.json");
            var setting = JsonConvert.DeserializeObject<SettingModel>(template);
            var sentry = new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(setting.SentryClientKey)
            });

            LogManager
               .SetDefaultLogCategory(setting.CategorySentryLog)
               .Use(new List<ILogEngine>
               {
                    sentry
               });
        }

        private static void ConfigureFanexData()
        {
            var storedProcedurePath = ConfigurationManager.AppSettings["StoredProcedurePath"];
            DbSettingProviderManager
                .StartNewSession()
                .UseDefaultConnectionStringProvider()
                .UseDefaultDbSettingProvider($@"~/{storedProcedurePath}")
                .Run();
        }
    }
}