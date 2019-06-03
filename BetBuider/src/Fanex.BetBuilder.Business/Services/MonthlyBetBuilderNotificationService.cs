using System;
using Fanex.BetBuilder.Business.Models;
using Fanex.MailDelivery.Core.Models;

namespace Fanex.BetBuilder.Business.Services
{
    public interface MonthlyBetBuilderNotificationService
    {
        JobMessage Execute();
    }

    public class MonthlyBetBuilderNotificationServiceImp : MonthlyBetBuilderNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly TransformService transformService;
        private readonly ExcelService excelService;
        private readonly MailService mailService;

        public MonthlyBetBuilderNotificationServiceImp(Log log, DataService dataService, TransformService transformService, MailService mailService, ExcelService excelService)
        {
            this.log = log;
            this.dataService = dataService;
            this.transformService = transformService;
            this.mailService = mailService;
            this.excelService = excelService;
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
            var rawData = dataService.QueryMonthlyData();
            var data = transformService.ExecuteMonthly(rawData);
            var isNoData = data.Count == 0;
            var fileName = isNoData ? string.Empty : excelService.Execute(data);
            var mail = mailService.SendMonthlyMail(fileName, isNoData);

            return new ExecuteResultModel { Success = true, Mail = mail };
        }
    }
}