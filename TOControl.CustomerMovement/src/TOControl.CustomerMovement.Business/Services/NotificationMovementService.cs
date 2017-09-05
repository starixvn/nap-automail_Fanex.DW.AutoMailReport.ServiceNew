using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace TOControl.CustomerMovement.Business.Services
{
    public interface NotificationMovementService
    {
        JobMessage Execute();
    }

    public class NotificationMovementServiceImp : NotificationMovementService
    {
        private readonly MailMovementService mailMovementService;
        private readonly LogService logService;
        private readonly DataService dataService;

        public NotificationMovementServiceImp(MailMovementService mailMovementService, LogService logService, DataService dataService)
        {
            this.mailMovementService = mailMovementService;
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
                    JobStatus.JobFailed,
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
            var mail = mailMovementService.Execute(data);
            return new ExecuteResult { Success = true, Mail = mail };
        }
    }
}