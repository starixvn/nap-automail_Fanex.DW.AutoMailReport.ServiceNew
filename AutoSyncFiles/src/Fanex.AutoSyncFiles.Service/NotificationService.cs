using Fanex.AutoSyncFiles.Business.Services;
using Quartz;
using Quartz.Impl;
using System;

namespace Fanex.AutoSyncFiles.Service
{
    public class NotificationService
    {
        private readonly Log logService;

        public NotificationService(
            Log logService
        )
        {
            this.logService = logService;
        }

        public bool Start()
        {
            Execute();
            return true;
        }

        private async void Execute()
        {
            try
            {
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();
                var job = JobBuilder.Create<NotificationJobImp>().Build();
                var trigger = TriggerBuilder.Create()
                .WithIdentity("Sync Files", "Sync Files")
                .StartNow()
                .WithCronSchedule(AppConfigService.AppConfig.CronSchedule)
                .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                logService.Log(ex);
            }
        }

        public bool Stop()
        {
            return true;
        }
    }
}