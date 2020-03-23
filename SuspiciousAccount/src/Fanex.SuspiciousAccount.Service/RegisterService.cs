using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.SuspiciousAccount.Business.Models;
using Fanex.SuspiciousAccount.Business.Services;
using Newtonsoft.Json;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Topshelf.SimpleInjector.QuickStart.Content;

namespace Fanex.SuspiciousAccount.Service
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            LoadConfig();
            SentryRegister();
            ConfigureFanexData();
        }

        public static void RegisterSimpleInjector(Container container)
        {
            container.Register<NotificationService>();
            container.Register<ClientApiService, ClientApiServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<IUMControlAdapter, UMControlAdapter>();
            container.Register<Log, LogImp>();            
            container.Register<EvidenceAccountService, EvidenceAccountServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<ApplicationSettingService, ApplicationSettingServiceImp>();
            container.Register<WebApiClientService, WebApiClientServiceImp>();
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("Templates/AppSettings.json");
            AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
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