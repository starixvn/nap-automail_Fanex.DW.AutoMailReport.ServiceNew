using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using TOControl.Notification.Business.Enums;

namespace TOControl.Notification.Business.Services
{
    public interface NotificationMovementService
    {
        JobMessage Execute(NotifiedByFunction notifiedByFunction);
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

        public JobMessage Execute(NotifiedByFunction notifiedByFunction)
        {
            var result = new ExecuteResult();
            JobMessage jobMessage;

            try
            {
                result = SendNotification(notifiedByFunction);
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
                    JobStatus.DataIsNotReady,
                    TaskLog = new TaskLog
                    {
                        LogTime = SystemTime.Now,
                        Message = result.Message                        
                    }
                };
            }

            return jobMessage;
        }

        private ExecuteResult SendNotification(NotifiedByFunction notifiedByFunction)
        {
            var data = dataService.Query(notifiedByFunction);
            var mail = mailMovementService.Execute(data);
            dataService.Update(notifiedByFunction);

            return new ExecuteResult { Success = true, Mail = mail };
        }
    }
}