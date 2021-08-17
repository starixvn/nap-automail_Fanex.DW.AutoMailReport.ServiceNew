using Fanex.AutoDelFiles.Business.Service;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Fanex.AutoDelFiles.Service
{
    public class NotificationJobImp : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<DelFileNotificationService>();
            service.Execute();

            var log = $"Fanex.AutoDelFiles.Service run at {DateTime.Now}{Environment.NewLine}";
            await Console.Out.WriteLineAsync(log);
        }
    }
}