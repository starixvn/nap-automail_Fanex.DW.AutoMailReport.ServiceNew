using Fanex.AutoDelFiles.Business.Models;
using Fanex.AutoDelFiles.Business.Service;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Newtonsoft.Json;
using SharpRaven;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.AutoDelFiles.Service
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
            var text = File.ReadAllText("Templates/DelFiles.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        public static void RegisterContainer(Container container)
        {
            container.Register<Log, LogImp>();
            container.Register<DelFileNotificationService, DelFileNotificationServiceImp>();
            container.Register<NotificationService>();
            container.Register<DelFileService, DelFileServiceImp>();
            container.Register<FactoryDelFileService, FactoryDelFileServiceImp>();

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