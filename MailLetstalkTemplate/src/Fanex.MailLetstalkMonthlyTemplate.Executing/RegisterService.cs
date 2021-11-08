using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.MailLetstalkTemplate.Business.Models;
using Fanex.MailLetstalkTemplate.Business.Services;
using Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.MailLetstalkTemplate.Executing
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

            var text = File.ReadAllText("Templates/Report.json");
            AppConfigService.Reports = JsonConvert.DeserializeObject<List<ReportModel>>(text);

            text = File.ReadAllText("Templates/BaseSetting.json");
            AppConfigService.BaseSetting = JsonConvert.DeserializeObject<BaseSetting>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DataService, DataServiceImp>();
            container.Register<MailLetstalkTemplateNotification, MailLetstalkTemplateNotificationImp>();
            container.Register(() => new ExcelPackage());
            container.Register<DateRangeServiceProfier, DateRangeServiceProfierImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ImageService, ImageServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<MailFormatProvider, MailFormatProviderImp>();
            container.Register<CsvService, CsvServiceImp>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<ApplicationSettingService, ApplicationSettingServiceImp>();
            container.Register<WebApiClientService, WebApiClientServiceImp>();
        }

        private static void SentryRegister()
        {
            var sentry = new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(AppConfigService.BaseSetting.SentryClientKey)
            });

            LogManager
               .SetDefaultLogCategory(AppConfigService.BaseSetting.CategorySentryLog)
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