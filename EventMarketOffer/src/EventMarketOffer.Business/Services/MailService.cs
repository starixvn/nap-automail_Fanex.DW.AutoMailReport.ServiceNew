using Fanex.MailDelivery.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace EventMarketOffer.Business.Services
{
    public interface MailService
    {
        Mail CreateMail();
    }

    public class MailServiceImp : MailService
    {
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly DateService dateService;

        private readonly AppConfigurationService appConfig;

        public MailServiceImp(
            FileService fileService,
            ExcelService excelService,
            AppConfigurationService appConfig,
            DateService dateService)
        {
            this.fileService = fileService;
            this.excelService = excelService;
            this.appConfig = appConfig;
            this.dateService = dateService;
        }

        public Mail CreateMail()
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
    }
}