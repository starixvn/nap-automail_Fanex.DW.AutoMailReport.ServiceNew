using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSports.Business.Models;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace BetradarVirtualSports.Business.Services
{
    public class MailServiceImp : MailService
    {
        private readonly AppConfig appConfig;

        public MailServiceImp(ApplicationConfigurationService appConfigService)
        {
            appConfig = appConfigService.Configurations;
        }

        public Mail Execute()
        {
            var fileName = @"Templates\Betradar_Virtual_Sport_Daily_Report.xlsx";
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} {SystemTime.Now:MM/dd/yyyy}",
                Title = appConfig.Title,
                Body = File.ReadAllText(@"Templates\ReadContentWhenDataAvailable-expected.html"),
                Attachments = attachments,
            };

            return mail;
        }

        public void SendMailWhenDataIsNotReady()
        {
            throw new NotImplementedException();
        }
    }
}