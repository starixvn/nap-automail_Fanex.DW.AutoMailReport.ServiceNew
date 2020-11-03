using Fanex.BigCreditIncrement789Y.Business.Services;
using ONEbook.UM.Services;
using Quartz;
using Quartz.Impl;
using System;

namespace Fanex.BigCreditIncrement789Y.Service
{
    public class NotificationService
    {
        private readonly IUMControlAdapter umcontrolAdapter;
        private readonly Log logService;

        public NotificationService(
            IUMControlAdapter umcontrolAdapter,
            Log logService
        )
        {
            this.umcontrolAdapter = umcontrolAdapter;
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
                if (umcontrolAdapter.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                {
                    return;
                }

                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();
                IJobDetail job = JobBuilder.Create<NotificationJobImp>().Build();
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("IDGJob", "IDG")
                .StartNow()
                .WithCronSchedule(AppConfigService.Config.CronSchedule)
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