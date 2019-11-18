using Fanex.AbnormalBets.Business.Models;
using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface MailService
    {
        bool IsMailSent(List<MailModel> mails, string mail);

        void SendMail(DateTime rptDate, string excelPath, bool isNoData, string subject, int mailId);
    }

    public class MailServiceImp : MailService
    {
        public bool IsMailSent(List<MailModel> mails, string mail)
        {
            if (AppConfigService.AppConfig.IsIgnoredCheckMailSent)
            {
                return false;
            }

            return mails.Any(s => s.MailName.ToLower().Equals(mail.ToLower()));
        }

        public void SendMail(DateTime rptDate, string excelPath, bool isNoData, string subject, int mailId)
        {
            var mail = new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Body = LoadBody(rptDate, isNoData),
                Subject = string.Format(subject, rptDate.ToString("MM/dd/yyyy")),
                Attachments = isNoData ? new List<MailAttachment>() : new List<MailAttachment> { new MailAttachment(excelPath, FileExtension.MS_EXCEL) },
                MailingListId = mailId
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }

        private string LoadBody(DateTime rptDate, bool isNoData)
        {
            var tuple = new Tuple<bool, DateTime>(isNoData, rptDate);
            var template = File.ReadAllText("Templates/NotificationEmail.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);
            return result;
        }
    }
}