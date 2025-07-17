using Fanex.DHWL.Business.Services;
using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Quartz;

namespace Fanex.DailyDHWL.Executing.Services
{
    public class ExecuteDailyDHWLService : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<DailyDHWLNotificationService>();
            var jobMessage = service.Execute();
            return jobMessage;
        }
    }
}