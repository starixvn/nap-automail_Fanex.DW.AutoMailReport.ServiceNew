using Fanex.AutoSyncFiles.Business.Services;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Fanex.AutoSyncFiles.Service
{
    [DisallowConcurrentExecution]
    public class NotificationJobImp : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var container = RegisterService.Container;
            var service = container.GetInstance<SyncFilesService>();
            service.Execute();

            var log = $"Fanex.AutoSyncFiles.Service run at {DateTime.Now}{Environment.NewLine}";
            await Console.Out.WriteLineAsync(log);
        }
    }
}