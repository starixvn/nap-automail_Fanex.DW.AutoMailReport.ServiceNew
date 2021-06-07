using Fanex.Caching;
using ONEbook.UM;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using SimpleInjector;
using System.Configuration;

namespace Fanex.CWLGamePayoutAlert.Service
{
    public static class OnebookUMInitializer
    {
        public static void InitializeUMClientManager()
        {
            var umConfig = GetUMClientConfiguration();

            UMClientManager
              .UseConfig(umConfig)
              .UseCacheService(new UMCacheService(new CacheService()))
              .UseRestClient(new UMRestClient(umConfig));
        }

        public static void RegisterSimpleInjector(Container container)
        {
            var umConfig = GetUMClientConfiguration();
            container.RegisterInstance(umConfig);
            container.RegisterSingleton<IHttpContextHelper, HttpContextHelper>();
            container.RegisterInstance<ICacheService>(new CacheService());
            container.RegisterSingleton<IUMCacheService, UMCacheService>();
            container.RegisterSingleton<IPHelper, IPHelperImpl>();
            container.RegisterSingleton<IUMRestClient, UMRestClient>();
            container.RegisterSingleton<IMaintenanceService, MaintenanceService>();
        }

        private static UMClientConfiguration GetUMClientConfiguration()
        {
            return new UMClientConfiguration(
                ConfigurationManager.AppSettings["ONEbook.UM:ServiceUri"],
                ConfigurationManager.AppSettings["ONEbook.UM:EnableCheckUM"] == "true",
                ConfigurationManager.AppSettings["ONEbook.UM:SecretKey"],
                ConfigurationManager.AppSettings["ONEbook.UM:EnableUnscheduledUM"] == "true",
                ConfigurationManager.AppSettings["ONEbook.UM:UMFrom"],
                ConfigurationManager.AppSettings["ONEbook.UM:UMTo"],
                ConfigurationManager.AppSettings["ONEbook.UM:UMPageUrl"],
                int.Parse(ConfigurationManager.AppSettings["ONEbook.UM:SiteId"]),
                int.Parse(ConfigurationManager.AppSettings["ONEbook.UM:ResultCacheDuration"]),
                ConfigurationManager.AppSettings["ONEbook.UM:UMLoggingCategory"],
                ConfigurationManager.AppSettings["ONEbook.UM:ErrorUMPageUrl"]
            );
        }
    }

    public class HttpContextHelper : IHttpContextHelper
    {
        public string GetSession(string key)
        {
            return string.Empty;
        }

        public string GetValueFromQueryString(string queryKey)
        {
            return string.Empty;
        }

        public void SetSession(string key, string value)
        {
        }
    }
}