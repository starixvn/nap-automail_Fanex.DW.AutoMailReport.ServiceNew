using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface MailService
    {
        void SendMailWhenDataIsNotReady(DateTime lastDate);
    }

    public class MailServiceImp : MailService
    {
        public void SendMailWhenDataIsNotReady(DateTime lastDate)
        {
            var mail = new Mail
            {
                Subject = $"[WARNING] - DATA IS NOT READY - Letstalk Report - Daily Soccer Performance",
                Title = "Auto Generated Report Administrator",
                Body = LoadContent(lastDate),
                Attachments = new List<MailAttachment>(),
                MailingListId = AppConfigService.AppConfig.FunctionIdNodata
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();            
        }

        private static string LoadContent(DateTime lastDate)
        {
            var tuple = new Tuple<DateTime>(lastDate);
            var template = File.ReadAllText(@"Templates/ContentWhenDataIsNotReady.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}
