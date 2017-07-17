using System.IO;
using NegativeBalance.Business.Models;
using Newtonsoft.Json;

namespace NegativeBalance.Business.Services
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