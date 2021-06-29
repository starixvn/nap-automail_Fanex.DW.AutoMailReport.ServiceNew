using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services;
using Fanex.DHWL.Business.Services.DailyDHWLLetstalk;
using Fanex.DHWL.Business.Services.LicenseePerformance;
using Fanex.DHWL.Business.Services.Margin;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Fanex.DailyDHWL.Executing
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
            var text = File.ReadAllText("Templates/DHWL.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);

            text = File.ReadAllText("Templates/ChartAreasPosition.json");
            AppConfigService.Config.AreasPosition = JsonConvert.DeserializeObject<Dictionary<int, ChartAreasPosition>>(text);

            text = File.ReadAllText("Templates/ImageSetting.json");
            AppConfigService.Config.ImageSettings = JsonConvert.DeserializeObject<Dictionary<string, ImageProperty>>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<BaseDataTable, BaseDataTableImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<DailyDHWLNotificationService, DailyDHWLNotificationServiceImp>();
            container.Register(() => new ExcelPackage());
            container.Register<ExcelRawService, ExcelRawServiceImp>();
            container.Register<DailyExcelService, DailyExcelServiceImp>();
            container.Register<ImageService, ImageServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DateTimeServiceProvider, DateTimeServiceProviderImp>();
            container.Register<DateTimeService, DateTimeLastMonthServiceImp>();
            container.Register<DataHouseIncomeService, DataHouseIncomeServiceImp>();
            container.Register<DataLastMonthService, DataLastMonthServiceImp>();
            container.Register<ChartService, ChartServiceImp>();
            container.Register<ChartDataService, ChartDataServiceImp>();
            container.Register<ChartExport, ChartExportImp>();
            container.Register<FormatService, FormatServiceImp>();
            container.Register<DataMarginService, DataMarginServiceImp>();
            container.Register<ExcelMarginService, ExcelMarginServiceImp>();
            container.Register<DailyDataDHWLService, DailyDataDHWLServiceImp>();
            container.Register<DailyExcelDHWLService, DailyExcelDHWLServiceImp>();
            container.Register<DailyImageDHWLService, DailyImageDHWLServiceImp>();
            container.Register<ImageMarginService, ImageMarginServiceImp>();
            container.Register<LicenseePerformanceDataService, LicenseePerformanceDataServiceImp>();
            container.Register<LicenseePerformanceExcelService, LicenseePerformanceExcelServiceImp>();
            container.Register<LicenseePerformanceService, LicenseePerformanceServiceImp>();
            container.Register<LicenseePerformanceImageService, LicenseePerformanceImageServiceImp>();

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