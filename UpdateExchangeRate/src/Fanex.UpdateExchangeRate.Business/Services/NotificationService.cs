using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using System;

namespace Fanex.UpdateExchangeRate.Business.Services
{
    public interface NotificationService
    {
        JobMessage Execute();
    }

    public class NotificationServiceImp : NotificationService
    {
        private readonly MailService mailService;
        private readonly Log log;
        private readonly DataService dataService;

        public NotificationServiceImp(MailService mailService, Log log, DataService dataService)
        {
            this.mailService = mailService;
            this.log = log;
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
                log.Log(exception);
            }
            finally
            {
                JobStatus jobStatus;
                if (string.IsNullOrEmpty(result.Message))
                {
                    jobStatus = result.Success ? JobStatus.Success : JobStatus.SuccessWithoutSendmail;
                }
                else
                {
                    jobStatus = JobStatus.DataIsNotReady;
                }

                jobMessage = new JobMessage
                {
                    Mail = result.Mail,
                    JobStatus = jobStatus,
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