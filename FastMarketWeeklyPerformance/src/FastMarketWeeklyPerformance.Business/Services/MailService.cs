using System.Collections.Generic;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public class MailServiceImp : MailService
    {
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly MailStationService mailStationService;

        private readonly AppConfig appConfig;

        public MailServiceImp(FileService fileService, ExcelService excelService, ApplicationConfigurationService appConfigService, MailStationService mailStationService)
        {
            this.fileService = fileService;
            this.excelService = excelService;
            this.mailStationService = mailStationService;

            appConfig = appConfigService.Configurations;
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
                Subject = $"{appConfig.Subject} - {SystemTime.Now:yyyy/MM/dd}",
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
                Subject = $"{appConfig.Subject} - {SystemTime.Now:yyyy/MM/dd}",
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