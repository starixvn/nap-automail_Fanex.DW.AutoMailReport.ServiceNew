using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.ESportModelingDataWeekly.Executing
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

            var text = File.ReadAllText("Templates/ESportModelingDataSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<ModelingDataNotificationService, ModelingDataNotificationServiceImp>();
            container.Register<ExportService, ExportServiceImp>();
            container.Register<CsvService, CsvServiceImp>();
            container.Register<ExportBaseService, ExportBaseServiceImp>();
            container.Register<ExportLevelFactory, ExportLevelFactoryImp>();
            container.Register<MailContentService, MailContentServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DateTimeServiceProvider, DateTimeServiceProviderImp>();
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<GenerateTemplateProvider, GenerateTemplateProviderImp>();
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