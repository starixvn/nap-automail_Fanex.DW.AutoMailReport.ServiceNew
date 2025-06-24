using Fanex.DHWL.Business.Services;
using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Quartz;

namespace Fanex.MonthlyDHWL.Executing.Services
{
    public class ExecuteMonthlyDHWLService : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<MonthlyDHWLNotificationService>();
            var jobMessage = service.Execute();
            return jobMessage;
        }
    }
}