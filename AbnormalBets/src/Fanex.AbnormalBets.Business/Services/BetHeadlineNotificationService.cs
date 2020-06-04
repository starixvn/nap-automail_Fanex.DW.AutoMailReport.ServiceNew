using Fanex.AbnormalBets.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface BetHeadlineNotificationService
    {
        JobMessage Execute();
    }

    public class BetHeadlineNotificationServiceImp : BetHeadlineNotificationService
    {
        private readonly Log log;
        private readonly RepositoryHeadlineService repositoryService;
        private readonly DateService dateService;
        private readonly TransformHeadlineService transformService;
        private readonly ExcelHeadlineService excelService;
        private readonly MailService mailService;

        public BetHeadlineNotificationServiceImp(
            Log log,
            RepositoryHeadlineService repositoryService,
            DateService dateService,
            TransformHeadlineService transformService,
            ExcelHeadlineService excelService,
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
            foreach (var group in AppConfigService.AppConfig.Groups)
            {
                var isSent = mailService.IsMailSent(mails, group.MailName);
                if (isSent)
                {
                    continue;
                }
                var rawData = repositoryService.QueryBetTickets(group.GroupName, date);
                var isNoData = !rawData.Item1.Any();
                var betData = transformService.ExecuteDailyLicensee(rawData);
                var excelPath = isNoData ? string.Empty : excelService.Execute(group.SiteName, date, betData);
                mailService.SendMail(date, excelPath, isNoData, group.Subject, group.MailId);
            }

            return new ExecuteResultModel { Success = true };
        }
    }
}