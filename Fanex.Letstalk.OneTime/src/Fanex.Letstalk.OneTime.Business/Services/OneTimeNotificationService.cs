using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
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
        private readonly LetstalkClientService letstalkService;
        private readonly MailService mailService;
        private readonly FileService fileService;

        public OneTimeNotificationServiceImp(
            Log log,
            MessageService messageService,
            DataService dataService,
            LetstalkClientService letstalkService,
            MailService mailService,
            FileService fileService)
        {
            this.log = log;
            this.messageService = messageService;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.mailService = mailService;
            this.fileService = fileService;
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
            var mails = dataService.QueryMail(DateTime.Today, JobStatus.SuccessWithoutSendmail);
            var mailSettings = AppConfigService.AppConfig.MailSettings["LetstalkOneTime"];
            var isMailSent = mails.FirstOrDefault(s => s.MailId == mailSettings.MailId) != null;
            bool sussess = false;
            if (isMailSent && !mailSettings.IsAllowResend)
            {
                message.Add($"{mailSettings.MailName} has been sent. Please be careful !!!");
                sussess = false;
            }
            else
            {
                message = messageService.SendMessage();
                sussess = message.Count == 0;
                if (!sussess)
                {
                    SendLetstalkNoData();
                }
            }

            return new ExecuteResultModel
            {
                Success = sussess,
                Mail = new Mail(),
                Message = string.Join(Environment.NewLine, message)
            };
        }

        private void SendLetstalkNoData()
        {
            var mailSetting = AppConfigService.AppConfig.MailSettings["CheckData"];
            var mails = dataService.QueryMail(DateTime.Today, JobStatus.Success);
            var isMailSent = mails.FirstOrDefault(s => s.MailId == mailSetting.MailId) != null;
            if (!isMailSent || mailSetting.IsAllowResend)
            {
                var content = LoadReportNameNotReady(false);
                var letstalkModel = new LetstalkModel
                {
                    TargetName = AppConfigService.AppConfig.TargetName,
                    Group = AppConfigService.AppConfig.Group,
                    Subject = "Data Is Not Ready",
                    Content = content,
                };

                letstalkService.SendMessage(letstalkModel);

                content = LoadReportNameNotReady(true);
                var mailModel = new Mail
                {
                    Title = "Auto Generated Report Administrator",
                    Subject = "Automail Report - Data Is Not Ready",
                    Body = content,
                    MailingListId = mailSetting.MailId
                };
                mailService.SendMail(mailModel);
            }
        }

        private string LoadReportNameNotReady(bool isMail)
        {
            var result = new List<string>();
            var content = fileService.LoadMessage(isMail);
            result.Add(content);
            var reportnames = messageService.GetReportNameNotReady();
            result.AddRange(reportnames);
            var breakLine = isMail ? "<br/>" : Environment.NewLine;

            return string.Join(breakLine + "- ", result);
        }
    }
}