using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.ServiceMonitoring.Models;
using Fanex.ServiceMonitoring.Services;
using Newtonsoft.Json;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using Sentry;
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
            container.Register<ServiceMonitoring>();
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