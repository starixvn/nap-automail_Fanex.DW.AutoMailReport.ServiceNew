using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.FinanceTransfer.Business.Models;
using Fanex.FinanceTransfer.Business.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.FinanceTransfer.Executing
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

            var text = File.ReadAllText("Templates/TransferSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<TransferNotificationService, TransferNotificationServiceImp>();
            container.Register<MailService, MailServiceImp>();
        }

        private static void SentryRegister()
        {
            var sentry = new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(AppConfigService.AppConfig.SentryClientKey)
            });

            LogManager
               .SetDefaultLogCategory(AppConfigService.AppConfig.CategorySentryLog)
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
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($@"~/{storedProcedurePath}")
                .Run();
        }
    }
}