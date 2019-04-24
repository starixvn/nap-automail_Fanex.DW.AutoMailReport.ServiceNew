using System;
using Fanex.MailDelivery.Core.Models;

namespace Fanex.Service.Core
{
    public interface DistributedNotificationService
    {
        JobMessage Execute();
    }

    public class DistributedNotificationServiceImp : DistributedNotificationService
    {
        private readonly MailService mailService;
        private readonly LogService logService;
        private readonly RepositoryService repositoryService;
        private JobMessage jobMessage;

        public DistributedNotificationServiceImp(MailService mailService, LogService logService, RepositoryService repositoryService)
        {
            this.mailService = mailService;
            this.logService = logService;
            this.repositoryService = repositoryService;
            jobMessage = new JobMessage();
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResult();

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

        private ExecuteResult SendNotification()
        {
            if (!repositoryService.IsDataAvailable())
            {
                mailService.SendMailWhenDataIsNotReady();
                return new ExecuteResult { Success = false, Message = "DataIsNotReady" };
            }

            var mail = mailService.Execute();
            return new ExecuteResult { Success = true, Mail = mail };
        }
    }
}