using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using TOControl.Notification.Business.Enums;
using System.Linq;

namespace TOControl.Notification.Business.Services
{
    public interface NotificationMovementService
    {
        JobMessage Execute(NotifiedByFunction notifiedByFunction);
    }

    public class NotificationMovementServiceImp : NotificationMovementService
    {
        private readonly MailMovementService mailMovementService;
        private readonly Log log;
        private readonly DataService dataService;

        public NotificationMovementServiceImp(MailMovementService mailMovementService, Log log, DataService dataService)
        {
            this.mailMovementService = mailMovementService;
            this.log = log;
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
                log.Log(exception);
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

        private ExecuteResult SendNotification(NotifiedByFunction notifiedByFunction)
        {
            var data = dataService.Query(notifiedByFunction);
            var isHasData = data.Item1.Any() || data.Item2.Any() || data.Item3.Any();
            var mail = new Mail();
            if (isHasData)
            {
                mail = mailMovementService.Execute(data);
                dataService.Update(notifiedByFunction);
            }

            return new ExecuteResult { Success = isHasData, Mail = mail };
        }
    }
}