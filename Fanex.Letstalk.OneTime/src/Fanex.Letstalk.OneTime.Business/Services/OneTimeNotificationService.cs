using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

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

        public OneTimeNotificationServiceImp(Log log, MessageService messageService)
        {
            this.log = log;
            this.messageService = messageService;
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
            var folderNotReady = messageService.SendMessage();
            return new ExecuteResultModel
            {
                Success = folderNotReady.Count == 0,
                Mail = new Mail(),
                Message = string.Join(Environment.NewLine, folderNotReady)
            };
        }
    }
}