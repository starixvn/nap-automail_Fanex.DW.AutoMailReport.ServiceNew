using Fanex.CWLGamePayoutAlert.Business.Services;
using ONEbook.UM.Services;
using Quartz;
using Quartz.Impl;
using System;

namespace Fanex.CWLGamePayoutAlert.Service
{
    public class NotificationService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly Log logService;

        public NotificationService(
            IMaintenanceService maintenanceService,
            Log logService
        )
        {
            this.maintenanceService = maintenanceService;
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
                if (maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                {
                    return;
                }

                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();
                IJobDetail job = JobBuilder.Create<NotificationJobImp>().Build();
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("CWL-Payout", "CWL")
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