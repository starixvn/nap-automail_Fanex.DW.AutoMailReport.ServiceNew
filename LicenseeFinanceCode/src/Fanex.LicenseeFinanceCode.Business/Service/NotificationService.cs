using Fanex.LicenseeFinanceCode.Business.Model;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.LicenseeFinanceCode.Business.Service
{
    public interface NotificationService
    {
        JobMessage Execute();
    }

    public class NotificationServiceImp : NotificationService
    {
        private readonly Log log;
        private readonly MailService mailService;
        private readonly DataService dataService;

        public NotificationServiceImp(Log log, MailService mailService, DataService dataService)
        {
            this.log = log;
            this.mailService = mailService;
            this.dataService = dataService;
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
            var rows = dataService.Query();
            return new ExecuteResultModel
            {
                Success = true,
                Mail = mailService.CreateMail(rows),
                Message = string.Empty
            };
        }
    }
}