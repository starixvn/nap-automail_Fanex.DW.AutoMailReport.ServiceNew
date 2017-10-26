using System.IO;
using Newtonsoft.Json;
using TOControl.Notification.Business.Models;

namespace TOControl.Notification.Business.Services
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