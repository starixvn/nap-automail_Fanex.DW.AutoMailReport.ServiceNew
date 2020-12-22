using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.MailLetstalkTemplate.Business.Models;
using Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface MailService
    {
        void SendMail(string excelPath);

        bool IsMailSent(List<MailModel> mails, string mail);
    }

    public class MailServiceImp : MailService
    {
        private readonly DateRangeServiceProfier dateRangeServiceProfier;
        private readonly MailFormatProvider mailFormatProvider;

        public MailServiceImp(DateRangeServiceProfier dateRangeServiceProfier, MailFormatProvider mailFormatProvider)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            this.mailFormatProvider = mailFormatProvider;
        }

        public bool IsMailSent(List<MailModel> mails, string mail)
        {
            if (AppConfigService.Config.IsIgnoredCheckMailSent)
            {
                return false;
            }

            return mails.Any(s => s.MailName.ToLower().Equals(mail.ToLower()));
        }

        public void SendMail(string excelPath)
        {
            var date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange);
            var mailFormat = mailFormatProvider.Format(AppConfigService.Config.DateRange);
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(excelPath, FileExtension.MS_EXCEL)
            };

            attachments = AppConfigService.Config.IsAllowAttachment ? attachments : new List<MailAttachment>();

            var mail = new Mail
            {
                Subject = string.Format(mailFormat.Subject()),
                Title = "ONEworks Auto Generated Report Administrator",
                Body = ReadBody(excelPath, date.ToDate),
                Attachments = attachments,
                MailingListId = AppConfigService.Config.MailId
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }

        private string ReadBody(string excelPath, DateTime toDate)
        {
            var shortName = new FileInfo(excelPath).Name;
            var tuple = new Tuple<string, DateTime>(shortName, toDate);
            var template = File.ReadAllText($"Templates/{AppConfigService.Config.EmailTemplate}");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}