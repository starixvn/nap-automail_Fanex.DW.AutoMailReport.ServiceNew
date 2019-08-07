using Fanex.MailDelivery.Core.Models;
using Fanex.ScoreBox.Business.Models;
using System;
using System.Linq;

namespace Fanex.ScoreBox.Business.Services
{
    public interface ScoreBoxNotifycationService
    {
        JobMessage Execute();
    }

    public class ScoreBoxNotifycationServiceImp : ScoreBoxNotifycationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelService excelService;
        private readonly MailScoreBoxService mailScoreBoxService;
        private readonly DateProvider dateProvider;

        public ScoreBoxNotifycationServiceImp(Log log, DataService dataService, ExcelService excelService, MailScoreBoxService mailScoreBoxService, DateProvider dateProvider)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelService = excelService;
            this.mailScoreBoxService = mailScoreBoxService;
            this.dateProvider = dateProvider;
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
            var isDaily = AppConfigService.AppConfig.IsDaily;
            var date = dateProvider.Date(isDaily);

            if (!dataService.IsDataAvailable())
            {
                var lastdate = dataService.QueryLastDate();
                mailScoreBoxService.SendMailWhenDataIsNotReady(isDaily, date.FromDate, date.ToDate, lastdate.LastDate);

                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var data = dataService.Execute();
            var isNoData = !data.Any();
            var fileName = excelService.Execute(isDaily, data);
            var mail = mailScoreBoxService.CreateMail(isDaily, date.FromDate, date.ToDate, fileName, isNoData);

            return new ExecuteResultModel { Success = true, Mail = mail };
        }
    }
}