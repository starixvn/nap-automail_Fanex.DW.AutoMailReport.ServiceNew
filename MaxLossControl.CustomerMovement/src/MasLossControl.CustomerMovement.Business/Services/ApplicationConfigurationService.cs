using System.IO;
using MaxLossControl.CustomerMovement.Business.Models;
using Newtonsoft.Json;

namespace MaxLossControl.CustomerMovement.Business.Services
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