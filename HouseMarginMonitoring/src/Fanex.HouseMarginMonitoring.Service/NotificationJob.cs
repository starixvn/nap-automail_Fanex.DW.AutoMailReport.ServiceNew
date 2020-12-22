using Fanex.HouseMarginMonitoring.Business.Services;
using Fanex.HouseMarginMonitoring.Service;
using Quartz;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fanex.HouseMarginMonitoring.Service
{
    public class NotificationJobImp : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<Business.Services.NotificationService>();
            service.Execute();

            var path = $@"{AppConfigService.Config.InternalPath}\Log.txt";

            var log = $"Fanex.HouseMarginMonitoring.Service run at {DateTime.Now}{Environment.NewLine}";
            File.AppendAllText(path, log);
            await Console.Out.WriteLineAsync(log);
        }
    }
}