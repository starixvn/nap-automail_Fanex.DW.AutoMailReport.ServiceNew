using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.ServiceMonitoring.Models;
using Quartz;
using Quartz.Spi;

namespace Fanex.ServiceMonitoring.Services
{
    public interface DetectMissingMailService
    {
        Dictionary<string, List<string>> QueryMail();
    }

    public class DetectMissingMailServiceImp : DetectMissingMailService
    {
        private readonly DataService dataService;

        public DetectMissingMailServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

        public Dictionary<string, List<string>> QueryMail()
        {
            var mails = new Dictionary<string, List<string>>();
            var tasks = dataService.QueryMailTasks();

            var missedMails = QueryMissedMail(tasks);
            var failedMails = QueryFailedMail(tasks);

            mails.Add("Missed Mails", FillterData(missedMails));
            mails.Add("Failed Mails", FillterData(failedMails));

            return mails;
        }

        private static List<string> FillterData(List<string> data)
        {
            if (AppConfigService.AppConfig.DataType == "UAT")
            {
                return data.Where(s => s.Contains("UAT")).ToList();
            }

            if (AppConfigService.AppConfig.DataType == "PRO")
            {
                var result = data.Where(s => AppConfigService.MailNeedToMonitor.Contains(s)).ToList();

                return result;
            }

            return data;
        }

        private List<string> QueryMissedMail(List<MailTaskModel> tasks)
        {
            var missedMails = new List<string>();
            var schedules = dataService.QuerySchedules();

            var fromDate = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.FromDate : DateTime.Today;
            var toDate = DateTime.Now;

            schedules
                .Select(s => s.MailId)
                .Except(tasks.Select(t => t.MailId))
                .ToList()
                .ForEach(missingMailId =>
                {
                    var schedule = schedules.FirstOrDefault(m => m.MailId == missingMailId);
                    if (schedule != null)
                    {
                        ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(schedule.CronExpression).Build();
                        var listtriggerdate = TriggerUtils.ComputeFireTimesBetween((IOperableTrigger)trigger, null, new DateTimeOffset(fromDate), new DateTimeOffset(toDate));
                        foreach (var triggertime in listtriggerdate)
                        {
                            var status = triggertime.LocalDateTime.AddSeconds(5) > DateTime.Now ? (int)JobStatus.Waiting : (int)JobStatus.Missing;
                            if (status == (int)JobStatus.Missing)
                            {
                                missedMails.Add(schedule.MailName);
                            }
                        }
                    }
                });

            return missedMails.Distinct().ToList();
        }

        private static List<string> QueryFailedMail(List<MailTaskModel> tasks)
        {
            var failedMails = new List<string>();
            var exceptMail = tasks
                .Where(s => s.Status == (int)JobStatus.Success || s.Status == (int)JobStatus.SuccessWithoutSendmail)
                .Select(s => s.MailId)
                .ToList();

            tasks
                .Where(s => s.Status == (int)JobStatus.JobFailed)
                .Select(s => s.MailId)
                .Except(exceptMail)
                .ToList()
                .ForEach(failedId =>
                {
                    var m = tasks.FirstOrDefault(f => f.MailId == failedId);
                    if (m != null)
                    {
                        failedMails.Add(m.MailName);
                    }
                });

            return failedMails;
        }
    }
}