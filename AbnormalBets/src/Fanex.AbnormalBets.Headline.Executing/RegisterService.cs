using Fanex.AbnormalBets.Business.Models;
using Fanex.AbnormalBets.Business.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.RemotingRef;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.AbnormalBets.Headline.Executing
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            LoadConfig();
            SentryRegister();
            ConfigureFanexData();
            RegisterRR();
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("Templates/AbnormalBetSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DateService, DateServiceImp>();
            container.Register<RepositoryHeadlineService, RepositoryHeadlineServiceImp>();
            container.Register<TransformHeadlineService, TransformHeadlineServiceImp>();
            container.Register<BetHeadlineNotificationService, BetHeadlineNotificationServiceImp>();
            container.Register<IRemotingRefService, RemotingRefService>();
            container.Register<BetteamService, BetteamServiceImp>();
            container.Register(() => new ExcelPackage());
            container.Register<ExcelHeadlineService, ExcelHeadlineServiceImp>();
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
                .UseDefaultConnectionStringProvider()
                .UseDefaultDbSettingProvider($@"~/{storedProcedurePath}")
                .Run();
        }

        private static void RegisterRR()
        {
            RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
                .FromApplication("Fanex.AbnormalBets.Service");
        }
    }
}