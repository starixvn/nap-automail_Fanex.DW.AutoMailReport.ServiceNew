using System.Collections.Generic;
using Fanex.MailDelivery.Core.Models;

namespace Fanex.BetBuilder.Business.Services
{
    public interface MailService
    {
        Mail SendDailyMail(string fileName, bool isNoData);

        Mail SendMonthlyMail(string fileName, bool isNoData);
    }

    public class MailServiceImp : MailService
    {
        private readonly MailContentService mailContentService;
        private readonly CriteriaService param;
        private readonly DateTimeService dateTimeService;

        public MailServiceImp(MailContentService mailContentService, CriteriaService param, DateTimeService dateTimeService)
        {
            this.mailContentService = mailContentService;
            this.param = param;
            this.dateTimeService = dateTimeService;
        }

        public Mail SendDailyMail(string fileName, bool isNoData)
        {
            var attachments = isNoData ? new List<MailAttachment>() : new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = $"[Bet Builder] - Daily Report {param.WinlostDate:MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = mailContentService.GenerateDailyMailContent(param.WinlostDate, isNoData),
                Attachments = attachments
            };

            return mail;
        }

        public Mail SendMonthlyMail(string fileName, bool isNoData)
        {
            var attachments = isNoData ? new List<MailAttachment>() : new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };
            var fromDate = dateTimeService.FromDate();
            var toDate = dateTimeService.ToDate();

            var mail = new Mail
            {
                Subject = $"[Bet Builder] - Monthly Cross Check Report {fromDate:MM/dd/yyyy} - {toDate:MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = mailContentService.GenerateMonthlyMailContent(fromDate, toDate, isNoData),
                Attachments = attachments
            };

            return mail;
        }
    }
}