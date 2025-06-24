using Fanex.DHWL.Business.Services;
using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Quartz;

namespace Fanex.MonthlyChart.Executing.Services
{
    public class ExecuteMonthlyChartDHWLService : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<MonthlyChartNotificationService>();
            var jobMessage = service.Execute();
            return jobMessage;
        }
    }
}