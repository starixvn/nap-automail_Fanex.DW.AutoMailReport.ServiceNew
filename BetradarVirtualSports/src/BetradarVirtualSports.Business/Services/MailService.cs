using BetradarVirtualSportsDaily.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System.Collections.Generic;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public interface MailService
    {
        Mail CreateMail();
    }

    public class MailServiceImp : MailService
    {
        private readonly AppConfig appConfig;
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly DateService dateService;

        public MailServiceImp(
            ApplicationConfigurationService appConfigService,
            FileService fileService,
            ExcelService excelService,
            DateService dateService)
        {
            appConfig = appConfigService.Configurations;
            this.fileService = fileService;
            this.excelService = excelService;
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
                Subject = $"{appConfig.Subject} {dateService.Date:MM/dd/yyyy}",
                Title = appConfig.Title,
                Body = fileService.ReadContentWhenDataIsAvailable(),
                Attachments = attachments,
            };

            return mail;
        }
    }
}