using System.IO;
using EventMarketOffer.Business.Models;
using Newtonsoft.Json;

namespace EventMarketOffer.Business.Services
{
    public interface AppConfigurationService
    {
        AppConfig Configurations { get; }
    }

    public class AppConfigurationServiceImp : AppConfigurationService
    {
        public AppConfig Configurations
        {
            get
            {
                var text = File.ReadAllText(@"Templates\AppConfig.json");
                return JsonConvert.DeserializeObject<AppConfig>(text);
            }
        }
    }
}