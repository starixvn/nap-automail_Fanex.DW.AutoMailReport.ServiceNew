using System.IO;
using Newtonsoft.Json;
using TopWinningAgent.Business.Models;

namespace TopWinningAgent.Business.Services
{
    public interface ApplicationConfigurationService
    {
        AppConfig Configurations { get; }
    }

    public class ApplicationConfigurationServiceImp : ApplicationConfigurationService
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