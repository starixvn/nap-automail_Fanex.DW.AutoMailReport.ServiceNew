using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.SoccerPerformance.Business.Models;
using System;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface SoccerPerformanceNotification
    {
        JobMessage Execute();
    }

    public class SoccerPerformanceNotificationImp : SoccerPerformanceNotification
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelService excelService;
        private readonly MessageService messageService;
        private readonly LetstalkClientService letstalkService;
        private readonly DateServiceProvider dateService;
        private readonly MailService mailService;

        public SoccerPerformanceNotificationImp(
            Log log,
            DateServiceProvider dateService,
            DataService dataService,
            ExcelService excelService,
            MessageService messageService,
            MailService mailService,
            LetstalkClientService letstalkService)
        {
            this.log = log;
            this.dateService = dateService;
            this.dataService = dataService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.mailService = mailService;
            this.letstalkService = letstalkService;
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
                jobMessage.Mail = new Mail();
                jobMessage.JobStatus = result.Success ? JobStatus.SuccessWithoutSendmail : JobStatus.DataIsNotReady;
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
                var lastDate = dataService.QueryLastDate();
                mailService.SendMailWhenDataIsNotReady(lastDate.LastDate);
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var date = dateService.Date;
            var startDate = date.Yesterday;
            var endDate = date.Yesterday;
            var yesterdayData = dataService.Query(startDate, endDate);
            var mtdData = dataService.Query(date.StartDate, date.EndDate);

            var fileNames = excelService.Execute(yesterdayData, mtdData);
            var letstalkModel = messageService.CreateLetstalkModel(fileNames);
            var result = letstalkService.SendMessage(letstalkModel);
            var message = string.Empty;
            if (!result)
            {
                message = "Letstalk send images unsuccessfully.";
            }

            return new ExecuteResultModel { Success = result, Message = message };
        }
    }
}