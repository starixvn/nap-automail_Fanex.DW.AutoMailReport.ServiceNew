using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.RemotingRef;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;

namespace Fanex.BetBuilder.Daily.Executing
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            LoadConfig();
            SentryRegister();
            ConfigureFanexData();
            RegisterRemotingRef();
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("Templates/DailyBetBuilderSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<CacheRefService, CacheRefServiceImp>();
            container.Register<IRemotingRefService, RemotingRefService>();
            container.Register<CriteriaService, CriteriaServiceImp>();
            container.Register<DailyBetBuilderNotificationService, DailyBetBuilderNotificationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DataService, DataServiceImp>();
            container.Register<TransformService, TransformServiceImp>();
            container.Register<CsvService, CsvServiceImp>();
            container.Register<MailContentService, MailContentServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DateTimeService, DateTimeServiceImp>();
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

        private static void RegisterRemotingRef()
        {
            RefDataTypeContainer
               .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
               .FromApplication("Fanex.BetBuilder.Service")
               .SupportOnlyDefaultLanguage();
        }
    }
}