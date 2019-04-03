using System;
using System.Collections.Generic;
using Fanex.MailDeliver.Model;
using Fanex.ServiceMonitoring.Models;

namespace Fanex.ServiceMonitoring.Services
{
    public interface MailService
    {
        void SendMail(List<ServiceModel> services);
    }

    public class MailServiceImp : MailService
    {
        private readonly MailContentService mailContentService;
        private readonly CommonService commonService;

        public MailServiceImp(MailContentService mailContentService, CommonService commonService)
        {
            this.mailContentService = mailContentService;
            this.commonService = commonService;
        }

        public void SendMail(List<ServiceModel> services)
        {
            if (services.Count == 0)
            {
                return;
            }

            using (var client = new MailDeliver.MailClient.API.MailDeliver())
            {
                var ip = commonService.GetLocalIP();
                var mail = new MailRecord
                {
                    Title = "Auto Generated Report Administrator",
                    Subject = $"Auto mail - Service Monitoring from Server {ip} - {DateTime.Now:yyyy-MM-dd hh:mm:ss tt}",
                    Body = mailContentService.GenerateMailContent(services),
                    MailTo = AppConfigService.AppConfig.MailTo
                };

                client.SendMail(mail);
            }
        }
    }
}