using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using Newtonsoft.Json;

namespace BetradarVirtualSportsDaily.Business.Services
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