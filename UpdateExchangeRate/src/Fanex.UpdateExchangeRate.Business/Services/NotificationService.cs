using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace Fanex.UpdateExchangeRate.Business.Services
{
    public interface NotificationService
    {
        JobMessage Execute();
    }

    public class NotificationServiceImp : NotificationService
    {
        private readonly MailService mailService;
        private readonly LogService logService;
        private readonly DataService dataService;

        public NotificationServiceImp(MailService mailService, LogService logService, DataService dataService)
        {
            this.mailService = mailService;
            this.logService = logService;
            this.dataService = dataService;
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResult();
            JobMessage jobMessage;
            try
            {
                result = SendNotification();
            }
            catch (Exception exception)
            {
                result.Success = false;
                result.Message = exception.Message;
                logService.Log(exception);
            }
            finally
            {
                jobMessage = new JobMessage
                {
                    Mail = result.Mail,
                    JobStatus = result.Success ?
                    JobStatus.Success :
                    JobStatus.SuccessWithoutSendmail,
                    TaskLog = new TaskLog
                    {
                        LogTime = SystemTime.Now,
                        Message = result.Message
                    }
                };
            }

            return jobMessage;
        }

        private ExecuteResult SendNotification()
        {
            var data = dataService.Query();
            if (data.Count == 0)
            {
                return new ExecuteResult { Success = false };
            }

            var mail = mailService.Execute(data);
            return new ExecuteResult { Success = true, Mail = mail };
        }
    }
}