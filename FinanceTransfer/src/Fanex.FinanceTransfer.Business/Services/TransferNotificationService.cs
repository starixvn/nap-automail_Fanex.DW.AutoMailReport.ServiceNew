using Fanex.FinanceTransfer.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.FinanceTransfer.Business.Services
{
    public interface TransferNotificationService
    {
        JobMessage Execute();
    }

    public class TransferNotificationServiceImp : TransferNotificationService
    {
        private readonly Log log;
        private readonly RepositoryService repositoryService;
        private readonly MailService mailService;

        public TransferNotificationServiceImp(Log log, RepositoryService repositoryService, MailService mailService)
        {
            this.log = log;
            this.repositoryService = repositoryService;
            this.mailService = mailService;
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
            var moveBetDay = repositoryService.SelMoveBetDay();
            var transferMonthDate = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.TransferMonthDate : null;
            var result = repositoryService.Execute(moveBetDay.MoveBetsDay, transferMonthDate);
            bool success = false;
            var mail = new Mail();
            if (result.ErrCode > 0)
            {
                mailService.SendMailToDBA(result.ErrMsg);
            }
            else
            {
                success = true;
                mail = mailService.CreateMail();
            }

            return new ExecuteResultModel { Success = success, Mail = mail, Message = result.ErrMsg };
        }
    }
}