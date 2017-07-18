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
        private readonly FileService fileService;

        public MailServiceImp(ApplicationConfigurationService appConfigService, FileService fileService)
        {
            appConfig = appConfigService.Configurations;
            this.fileService = fileService;
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
                Body = fileService.ReadContentWhenDataIsAvailable(),
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