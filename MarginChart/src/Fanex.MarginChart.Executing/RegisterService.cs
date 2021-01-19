using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.MarginChart.Business.Models;
using Fanex.MarginChart.Business.Services;
using Fanex.MarginChart.Business.Services.Margin;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.MarginChart.Executing
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
            var text = File.ReadAllText("Templates/MarginChart.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);

            text = File.ReadAllText("Templates/ChartAreasPosition.json");
            AppConfigService.Config.AreasPosition = JsonConvert.DeserializeObject<Dictionary<int, ChartAreasPosition>>(text);

            text = File.ReadAllText("Templates/ImageSetting.json");
            AppConfigService.Config.ImageSettings = JsonConvert.DeserializeObject<Dictionary<string, ImageProperty>>(text);

            text = File.ReadAllText("Templates/ProductSettings.json");
            AppConfigService.Config.ProductGroups = JsonConvert.DeserializeObject<List<ProductGroup>>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<BaseDataTable, BaseDataTableImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<DailyMarginChartNotificationService, DailyMarginChartNotificationServiceImp>();
            container.Register(() => new ExcelPackage());
            container.Register<ExcelRawService, ExcelRawServiceImp>();
            container.Register<ImageService, ImageServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<DateTimeServiceProvider, DateTimeServiceProviderImp>();
            container.Register<ChartService, ChartServiceImp>();
            container.Register<FormatService, FormatServiceImp>();
            container.Register<ExcelMarginService, ExcelMarginServiceImp>();
            container.Register<DataMarginService, DataMarginServiceImp>();
            container.Register<ImageMarginService, ImageMarginServiceImp>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<ApplicationSettingService, ApplicationSettingServiceImp>();
            container.Register<WebApiClientService, WebApiClientServiceImp>();
        }

        private static void SentryRegister()
        {
            var sentry = new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(AppConfigService.Config.SentryClientKey)
            });

            LogManager
               .SetDefaultLogCategory(AppConfigService.Config.CategorySentryLog)
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