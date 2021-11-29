using Fanex.MailDelivery.Core.Models;
using FastMarketWeeklyPerformance.Business.Models;
using System;
using System.Collections.Generic;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface MailService
    {
        Mail CreateMail();
    }

    public class MailServiceImp : MailService
    {
        private readonly FileService fileService;
        private readonly ExcelService excelService;

        private readonly AppConfig appConfig;

        public MailServiceImp(
            FileService fileService,
            ExcelService excelService,
            ApplicationConfigurationService appConfigService)
        {
            this.fileService = fileService;
            this.excelService = excelService;

            appConfig = appConfigService.Configurations;
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
                Subject = $"{appConfig.Subject} - {DateTime.Now:yyyy/MM/dd}",
                Title = appConfig.Title,
                Body = fileService.ReadContentWhenDataIsAvailable(),
                Attachments = attachments,
            };

            return mail;
        }
    }
}