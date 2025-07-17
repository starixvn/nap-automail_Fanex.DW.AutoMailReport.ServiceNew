using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Fanex.MarginChart.Business.Services;
using Quartz;

namespace Fanex.MarginChart.Executing.Services
{
    public class ExecuteMarginChartService : JobBase
    {
        protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<DailyMarginChartNotificationService>();
            var jobMessage = service.Execute();
            return jobMessage;
        }
    }
}