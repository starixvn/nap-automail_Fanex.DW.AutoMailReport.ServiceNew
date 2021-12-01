using Fanex.MailDelivery.Core.Models;
using System;
using TopWinningAgent.Business.Models;

namespace TopWinningAgent.Business.Services
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

        public NotificationServiceImp(
            MailService mailService,
            Log log,
            DataService dataService)
        {
            this.mailService = mailService;
            this.log = log;
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
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                log.Log(ex);
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
            if (!dataService.IsDataAvailable())
            {
                return new ExecuteResultModel
                {
                    Success = false,
                    Message = "DataIsNotReady, TaskID = 21"
                };
            }

            var mail = mailService.CreatMail();

            return new ExecuteResultModel
            {
                Mail = mail,
                Success = true,
                Message = string.Empty
            };
        }
    }
}