using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.TopLicensee.Service.Models;
using Fanex.TopLicensee.Service.Services;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.TopLicenseePerformance.Executing
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

            var text = File.ReadAllText("Templates/TopLicenseePerformanceSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DataService, DataServiceImp>();
            container.Register<TopLicenseeNotificationService, TopLicenseeNotificationServiceImp>();
            container.Register<ExcelTop50Service, ExcelTop50ServiceImp>();
            container.Register<ExcelTopLicenseeMarginService, ExcelTopLicenseeMarginServiceImp>();
            container.Register<ImageService, ImageServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<ExportVirtualSoccerHouse, ExportVirtualSoccerHouseImp>();
            container.Register<ExportVirtualSoccerLicensee, ExportVirtualSoccerLicenseeImp>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<ApplicationSettingService, ApplicationSettingServiceImp>();
            container.Register<WebApiClientService, WebApiClientServiceImp>();
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
    }
}