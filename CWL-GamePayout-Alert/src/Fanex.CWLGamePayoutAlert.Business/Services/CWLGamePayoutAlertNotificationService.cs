using Fanex.CWLGamePayoutAlert.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.CWLGamePayoutAlert.Business.Services
{
    public interface CWLGamePayoutAlertNotificationService
    {
        JobMessage Execute();
    }

    public class CWLGamePayoutAlertNotificationServiceImp : CWLGamePayoutAlertNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly MailService mailService;
        private readonly DateTimeServiceProvider dateService;

        private Mail mail = new Mail { Subject = string.Empty, Body = string.Empty };

        public CWLGamePayoutAlertNotificationServiceImp(
            Log log,
            DataService dataService,
            MailService mailService,
            DateTimeServiceProvider dateService)
        {
            this.log = log;
            this.dataService = dataService;
            this.mailService = mailService;
            this.dateService = dateService;
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
            var data = dataService.QueryPayout();
            var fromGMT8Time = dateService.DateTime.FromGMT8Time;
            mailService.SendMail(fromGMT8Time, data);

            return new ExecuteResultModel
            {
                Success = true,
                Message = string.Empty,
                Mail = mail
            };
        }
    }
}