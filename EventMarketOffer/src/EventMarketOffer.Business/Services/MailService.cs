using System.Collections.Generic;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace EventMarketOffer.Business.Services
{
    public class MailServiceImp : MailService
    {
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly MailStationService mailStationService;
        private readonly DateService dateService;

        private readonly AppConfigurationService appConfig;

        public MailServiceImp(FileService fileService, ExcelService excelService, AppConfigurationService appConfig, MailStationService mailStationService, DateService dateService)
        {
            this.fileService = fileService;
            this.excelService = excelService;
            this.appConfig = appConfig;
            this.mailStationService = mailStationService;
            this.dateService = dateService;
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
                Subject = $"{appConfig.Configurations.Subject} from {dateService.DateStart:dd MMM yyyy} to {dateService.DateEnd:dd MMM yyyy}",
                Title = appConfig.Configurations.Title,
                Body = fileService.ReadContentWhenDataIsAvailable(new FileInfo(fileName).Name),
                Attachments = attachments,
            };

            return mail;
        }

        public void SendMailWhenDataIsNotReady()
        {
            var mail = new Mail
            {
                Subject = $"{appConfig.Configurations.Subject} from {dateService.DateStart:dd MMM yyyy} to {dateService.DateEnd:dd MMM yyyy}",
                Title = appConfig.Configurations.Title,
                Body = fileService.ReadContentWhenDataIsNotReady(),
                Attachments = new List<MailAttachment>(),
                MailingListId = appConfig.Configurations.FunctionIdNodata
            };

            if (appConfig.Configurations.FunctionIdNodata > 0)
            {
                mailStationService.Send(mail);
            }
        }
    }
}