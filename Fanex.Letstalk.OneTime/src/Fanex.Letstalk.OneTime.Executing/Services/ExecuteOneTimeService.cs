using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.Letstalk.OneTime.Business.Services;
using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Newtonsoft.Json;
using Quartz;
using System.IO;

namespace Fanex.Letstalk.OneTime.Executing.Services
{
    public class ExecuteOneTimeService : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<OneTimeNotificationService>();
            var text = File.ReadAllText("Templates/LetstalkOneTimeSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
            var jobMessage = service.Execute();
            return jobMessage;
        }
    }
}