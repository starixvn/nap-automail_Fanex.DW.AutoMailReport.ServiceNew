using Fanex.CN88DailyPerformance.Service.Model;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.CN88DailyPerformance.Service.Services
{
    public interface NotificationService
    {
        JobMessage Execute();
    }

    public class NotificationServiceImp : NotificationService
    {
        private readonly Log log;
        private readonly DateService dateService;
        private readonly DataService dataService;
        private readonly ExcelService excelService;
        private readonly ConfigSettingModel config;
        private readonly MailService mailService;


        public NotificationServiceImp(
            Log log,
            DateService dateService,
            DataService dataService,
            ExcelService excelService,
            MailService mailService)
        {
            this.log = log;
            this.dateService = dateService;
            this.dataService = dataService;
            this.excelService = excelService;
            this.mailService = mailService;
            config = AppConfigService.AppConfig;
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new JobMessage();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.Success : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            var reportDate = dateService.GetYesterday();
            var dailyPerformances = dataService.Query(reportDate);
            var excelFiles = excelService.ExportExcel(dailyPerformances, reportDate);
            var mail = mailService.CreateMail(excelFiles);

            return new ExecuteResultModel { Success = true, Mail = mail };
        }
    }
}
