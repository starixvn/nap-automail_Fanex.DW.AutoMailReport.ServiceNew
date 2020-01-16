using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;

namespace Fanex.TopLicensee.Service.Services
{
    public interface MailService
    {
        void SendMailWhenDataIsNotReady();
    }

    public class MailServiceImp : MailService
    {
        private readonly DataService dataService;

        public MailServiceImp(DataService dataService)
        {
            this.dataService = dataService;
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

        private string LoadBodyMailNoData()
        {
            var tuple = new Tuple<DateTime>(dataService.QueryLastDate().LastDate);
            var template = File.ReadAllText("Templates/ContentWhenDataIsNotReady.cshtml");
            return Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);
        }
    }
}