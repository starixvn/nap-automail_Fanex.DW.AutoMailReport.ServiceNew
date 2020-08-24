using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Letstalk.OneTime.Business.Services
{
    public interface OneTimeNotificationService
    {
        JobMessage Execute();
    }

    public class OneTimeNotificationServiceImp : OneTimeNotificationService
    {
        private readonly Log log;
        private readonly MessageService messageService;
        private readonly DataService dataService;

        public OneTimeNotificationServiceImp(Log log, MessageService messageService, DataService dataService)
        {
            this.log = log;
            this.messageService = messageService;
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
            var message = new List<string>();
            var mails = dataService.QueryMail(DateTime.Today);
            var isMailSent = mails.FirstOrDefault(s => s.MailId == AppConfigService.AppConfig.MailId) != null;
            bool sussess = false;
            if (isMailSent && !AppConfigService.AppConfig.IsAllowResend)
            {
                message.Add($"{AppConfigService.AppConfig.MailName} has been sent. Please be careful !!!");
                sussess = false;
            }
            else
            {
                message = messageService.SendMessage();
                sussess = message.Count == 0;
            }

            return new ExecuteResultModel
            {
                Success = sussess,
                Mail = new Mail(),
                Message = string.Join(Environment.NewLine, message)
            };
        }
    }
}