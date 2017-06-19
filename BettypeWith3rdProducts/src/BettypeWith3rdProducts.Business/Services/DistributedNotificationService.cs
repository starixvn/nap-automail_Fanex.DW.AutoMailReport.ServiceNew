using System;
using BettypeWith3rdProducts.Business.Models;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface DistributedNotificationService
    {
        JobMessage Execute();
    }

    public class DistributedNotificationServiceImp : DistributedNotificationService
    {
        private readonly MailService mailService;
        private readonly LogService logService;
        private readonly DataService dataService;

        public DistributedNotificationServiceImp(MailService mailService, LogService logService, DataService dataService)
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
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                logService.Log(e);
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
            if (!dataService.IsDataAvailable())
            {
                mailService.SendMailWhenDataIsNotReady();
                return new ExecuteResult { Success = false, Message = "DataIsNotReady" };
            }

            var mail = mailService.Execute();
            return new ExecuteResult { Success = true, Mail = mail };
        }
    }
}