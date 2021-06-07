using Fanex.CWLGamePayoutAlert.Business.Services;
using Fanex.Dangerous789YCustomer.Service;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Fanex.CWLGamePayoutAlert.Service
{
    public class NotificationJobImp : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<CWLGamePayoutAlertNotificationService>();
            service.Execute();

            var log = $"Fanex.CWLGamePayoutAlert.Service run at {DateTime.Now}{Environment.NewLine}";
            await Console.Out.WriteLineAsync(log);
        }
    }
}