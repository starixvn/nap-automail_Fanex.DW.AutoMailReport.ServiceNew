using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.ServiceMonitoring.Models;
using Fanex.ServiceMonitoring.Services;
using Newtonsoft.Json;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using SharpRaven;
using SimpleInjector;

namespace Fanex.ServiceMonitoring.Service
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            LoadConfig();
            ConfigureFanexData();
            SentryRegister();
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<IUMControlAdapter, UMControlAdapter>();
            container.Register<DataService, DataServiceImp>();
            container.Register<RestartService, RestartServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<Log, LogImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<MailContentService, MailContentServiceImp>();
            container.Register<CommonService, CommonServiceImp>();
            container.Register<DetectMissingMailService, DetectMissingMailServiceImp>();
            container.Register<MessageContentService, MessageContentServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<ServiceMonitoring>();
            container.Register<MailMonitoring>();
            container.Register<StartInstance>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<ApplicationSettingService, ApplicationSettingServiceImp>();
            container.Register<WebApiClientService, WebApiClientServiceImp>();
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

        private static void LoadConfig()
        {
            var text = File.ReadAllText("Templates/MonitoringSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);

            text = File.ReadAllText("Templates/MailNeedToMonitor.json");
            AppConfigService.MailNeedToMonitor = JsonConvert.DeserializeObject<List<string>>(text);
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
    }
}