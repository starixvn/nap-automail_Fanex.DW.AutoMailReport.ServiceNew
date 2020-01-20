using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.CN88DailyPerformance.Service.Services
{
    public interface MailService
    {
        Mail CreateMail(string excelFile);
    }

    public class MailServiceImp : MailService
    {
        public Mail CreateMail(string excelFile)
        {
            var mailAttachments = new List<MailAttachment>();
            mailAttachments.Add(new MailAttachment(excelFile, FileExtension.MS_EXCEL));

            return new Mail
            {
                Title = "Auto Generated Report Administrator",
                Subject = $"CN88 Daily Performance Report - {SystemTime.Now.AddDays(-1):yyyy/MM/dd}",
                Body = LoadBody(excelFile),
                Attachments = mailAttachments,
            };
        }

        private string LoadBody(string excelFile)
        {
            var fileName = new FileInfo(excelFile).Name;
            var template = File.ReadAllText("Templates/NotificationEmail.cshtml");

            return Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: fileName);
        }

    }
}
