using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.DHWL.Business.Services
{
    public interface MailService
    {
        void SendMail(string fileName, string excelTitle, string subject, DateTime rptDate);
    }

    public class MailServiceImp : MailService
    {
        public void SendMail(string fileName, string excelTitle, string subject, DateTime rptDate)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = $"{subject} At {rptDate:yyyy/MM/dd HH:00}",
                Body = Content(fileName, excelTitle, rptDate),
                Attachments = attachments,
                MailingListId = AppConfigService.Config.MailId,
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }

        private string Content(string fileName, string excelTitle, DateTime rptDate)
        {
            var tuple = new Tuple<string, string, DateTime>(new FileInfo(fileName).Name, excelTitle, rptDate);
            var template = File.ReadAllText("Templates/DHWL.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}