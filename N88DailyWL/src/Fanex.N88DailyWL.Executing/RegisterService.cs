using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.N88DailyWL.Business.Models;
using Fanex.N88DailyWL.Business.Services;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.N88DailyWL.Executing
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

            var text = File.ReadAllText("Templates/N88Settings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DataService, DataServiceImp>();
            container.Register<N88NotificationService, N88NotificationServiceImp>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\N88Template.xlsx")));
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<ImageService, ImageServiceImp>();
            container.Register<MessageService, MessageServiceImp>();

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