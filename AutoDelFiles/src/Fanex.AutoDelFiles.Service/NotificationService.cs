using Fanex.AutoDelFiles.Business.Service;
using Quartz;
using Quartz.Impl;
using System;

namespace Fanex.AutoDelFiles.Service
{
    public class NotificationService
    {
        private readonly Log logService;

        public NotificationService(Log logService)
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
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();
                IJobDetail job = JobBuilder.Create<NotificationJobImp>().Build();
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Del Files", "Del Files")
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