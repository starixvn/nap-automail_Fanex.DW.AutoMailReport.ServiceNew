using System.Collections.Generic;
using BetradarVirtualSportsDaily.Business.Models;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public class MailDailyService : MailService
    {
        private readonly AppConfig appConfig;
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly DateService dateService;
        private readonly MailStationService mailStationService;

        public MailDailyService(ApplicationConfigurationService appConfigService, FileService fileService, ExcelService excelService, DateService dateService, MailStationService mailStationService)
        {
            appConfig = appConfigService.Configurations;
            this.fileService = fileService;
            this.excelService = excelService;
            this.dateService = dateService;
            this.mailStationService = mailStationService;
        }

        public Mail Execute()
        {
            var fileName = excelService.Export().Save();
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} {dateService.Date:MM/dd/yyyy}",
                Title = appConfig.Title,
                Body = fileService.ReadContentWhenDataIsAvailable(),
                Attachments = attachments,
            };

            return mail;
        }

        public void SendMailWhenDataIsNotReady()
        {
            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} {dateService.Date:MM/dd/yyyy}",
                Title = appConfig.Title,
                Body = fileService.ReadContentWhenDataIsNotReady(),
                Attachments = new List<MailAttachment>(),
                MailingListId = appConfig.FunctionIdNodata
            };

            if (appConfig.FunctionIdNodata > 0)
            {
                mailStationService.Send(mail);
            }
        }
    }
}