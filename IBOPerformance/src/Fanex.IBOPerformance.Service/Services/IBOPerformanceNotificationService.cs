using Fanex.IBOPerformance.Service.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.IBOPerformance.Service.Services
{
    public interface IBOPerformanceNotificationService
    {
        JobMessage Execute();
    }

    public class IBOPerformanceNotificationServiceImp : IBOPerformanceNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelService excelService;
        private readonly ConfigSettingModel config;
        private readonly MailService mailService;

        public IBOPerformanceNotificationServiceImp(Log log, DataService dataService, ExcelService excelService, MailService mailService)
        {
            this.log = log;
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
            if (!dataService.IsDataAvailable())
            {
                mailService.SendMailWhenDataIsNotReady();
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var createdCustomers = dataService.QueryDailyCreatedCustomer(config.Site, config.CurrencyId);
            var performanceCustomers = dataService.QueryDailyPerformance(config.Site, config.CurrencyId);
            var excelFiles = excelService.ExportExcel(createdCustomers, performanceCustomers);
            var mail = mailService.CreateMail(excelFiles);

            return new ExecuteResultModel { Success = true, Mail = mail };
        }
    }
}