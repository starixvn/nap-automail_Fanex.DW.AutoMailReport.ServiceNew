using Fanex.MailDelivery.Core.Models;
using System.Collections.Generic;
using TopWinningAgent.Business.Models;

namespace TopWinningAgent.Business.Services
{
    public interface MailService
    {
        Mail CreatMail();
    }

    public class MailServiceImp : MailService
    {
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly DateRangeService dateRangeService;

        private readonly AppConfig appConfig;

        public MailServiceImp(
            FileService fileService,
            ExcelService excelService,
            ApplicationConfigurationService appConfigService,
            DateRangeService dateRangeService)
        {
            this.fileService = fileService;
            this.excelService = excelService;
            this.dateRangeService = dateRangeService;

            appConfig = appConfigService.Configurations;
        }

        public Mail CreatMail()
        {
            var fileName = excelService.Export().Save();
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} {dateRangeService.FromDate(1): MMM dd, yyyy} to {dateRangeService.ToDate():MMM dd, yyyy}",
                Title = appConfig.Title,
                Body = fileService.ReadContent(),
                Attachments = attachments,
            };

            return mail;
        }
    }
}