using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.Letstalk.OneTime.Business.Services;
using Fanex.LetstalkService.Client;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.Mattermost.Client;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using AppConfigService = Fanex.Letstalk.OneTime.Business.Services.AppConfigService;
using LetstalkConfig = Fanex.LetstalkService.Core.Services.AppConfigService;

namespace Fanex.Letstalk.OneTime.Executing
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            ConfigureFanexData();
            LoadConfig();
            SentryRegister();
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("Templates/LetstalkOneTimeSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            LetstalkConfig.Config.LetstalkServiceApi = AppConfigService.AppConfig.LetstalkServiceApi;
            Mattermost.Client.AppConfigService.Config.MattermostServiceApi = AppConfigService.AppConfig.MattermostUrl;
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<OneTimeNotificationService, OneTimeNotificationServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<MailService, MailServiceImp>();
            container.Register<FileService, FileServiceImp>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<MattermostClientService, MattermostClientServiceImp>();
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