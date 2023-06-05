using Fanex.AutoSyncFiles.Business.Models;
using Fanex.AutoSyncFiles.Business.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.AutoSyncFiles.Service
{
    public static class RegisterService
    {
        public static void RegisterServices()
        {
            LoadConfig();
            SentryRegister();
        }

        private static void LoadConfig()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var text = File.ReadAllText("Templates/SyncFiles.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.RegisterSingleton<Log, LogImp>();
            container.RegisterSingleton<SyncFilesService, SyncFilesServiceImp>();
            container.RegisterSingleton<MailService, MailServiceImp>();
            container.RegisterSingleton<NotificationService>();

            Container = container;
        }

        public static Container Container { get; set; }

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