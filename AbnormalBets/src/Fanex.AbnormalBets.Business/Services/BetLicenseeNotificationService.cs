using Fanex.AbnormalBets.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface BetLicenseeNotificationService
    {
        JobMessage Execute();
    }

    public class BetLicenseeNotificationServiceImp : BetLicenseeNotificationService
    {
        private readonly Log log;
        private readonly RepositoryLicenseeService repositoryService;
        private readonly DateService dateService;
        private readonly TransformLicenseeService transformService;
        private readonly ExcelLicenseeService excelService;
        private readonly MailService mailService;

        public BetLicenseeNotificationServiceImp(
            Log log,
            RepositoryLicenseeService repositoryService,
            DateService dateService,
            TransformLicenseeService transformService,
            ExcelLicenseeService excelService,
            MailService mailService)
        {
            this.log = log;
            this.repositoryService = repositoryService;
            this.dateService = dateService;
            this.transformService = transformService;
            this.excelService = excelService;
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
            var date = dateService.Date();
            var mails = repositoryService.QueryMail(date);
            foreach (var site in AppConfigService.AppConfig.Sites)
            {
                var isSent = mailService.IsMailSent(mails, site.MailName);
                if (isSent)
                {
                    continue;
                }
                var rawData = repositoryService.QueryBetTickets(site.SiteName, date);
                var isNoData = !rawData.Item1.Any();
                var betData = transformService.ExecuteDailyLicensee(rawData);
                var excelPath = isNoData ? string.Empty : excelService.Execute(site.SiteName, date, betData);
                mailService.SendMail(date, excelPath, isNoData, site.Subject, site.MailId);
            }

            return new ExecuteResultModel { Success = true };
        }
    }
}