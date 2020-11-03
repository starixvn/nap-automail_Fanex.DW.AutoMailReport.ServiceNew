using Fanex.BigCreditIncrement789Y.Business.Services;
using Fanex.Dangerous789YCustomer.Service;
using Quartz;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fanex.BigCreditIncrement789Y.Service
{
    public class NotificationJobImp : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<BigCreditIncrement789YNotificationService>();
            service.Execute();

            var path = $@"{AppConfigService.Config.InternalPath}\Log.txt";

            var log = $"Fanex.BigCreditIncrement789Y.Service run at {DateTime.Now}{Environment.NewLine}";
            File.AppendAllText(path, log);
            await Console.Out.WriteLineAsync(log);
        }
    }
}