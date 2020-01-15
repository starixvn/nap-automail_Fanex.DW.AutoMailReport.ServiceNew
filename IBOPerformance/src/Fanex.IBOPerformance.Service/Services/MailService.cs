using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.IBOPerformance.Service.Services
{
    public interface MailService
    {
        Mail CreateMail(List<string> excelFiles);

        void SendMailWhenDataIsNotReady();
    }

    public class MailServiceImp : MailService
    {
        private readonly DataService dataService;

        public MailServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

        public Mail CreateMail(List<string> excelFiles)
        {
            var mailAttachments = new List<MailAttachment>();
            excelFiles.ForEach(file =>
            {
                mailAttachments.Add(new MailAttachment(file, FileExtension.MS_EXCEL));
            });

            return new Mail
            {
                Title = "Auto Generated Report Administrator",
                Subject = $"Automail Report - Daily IBO Customer Statistics Report - {SystemTime.Now.AddDays(-1):yyyy/MM/dd}",
                Body = LoadBody(excelFiles),
                Attachments = mailAttachments,
            };
        }

        public void SendMailWhenDataIsNotReady()
        {
            if (AppConfigService.AppConfig.FunctionIdNodata > 0)
            {
                var mail = new Mail
                {
                    Title = "Auto Generated Report Administrator",
                    Subject = $"[WARNING] - DATA IS NOT READY - Automail Report - Daily IBO Customer Statistics Report - {SystemTime.Now.AddDays(-1):yyyy/MM/dd}",
                    Body = LoadBodyMailNoData(),
                    MailingListId = AppConfigService.AppConfig.FunctionIdNodata
                };

                MailClientHandler.Start();
                MailClientHandler.SendFunctionMail(mail);
                MailClientHandler.Stop();
            }
        }

        private string LoadBody(List<string> excelFiles)
        {
            var shotnames = new List<string>();
            excelFiles.ForEach(file =>
            {
                shotnames.Add(new FileInfo(file).Name);
            });

            var tuple = new Tuple<List<string>>(shotnames);
            var template = File.ReadAllText("Templates/NotificationEmail.cshtml");
            return Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);
        }

        private string LoadBodyMailNoData()
        {
            var tuple = new Tuple<DateTime>(dataService.QueryLastDate().LastDate);
            var template = File.ReadAllText("Templates/ContentWhenDataIsNotReady.cshtml");
            return Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);
        }
    }
}