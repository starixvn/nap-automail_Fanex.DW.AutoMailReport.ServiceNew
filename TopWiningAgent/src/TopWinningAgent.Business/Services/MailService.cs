using System.Collections.Generic;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using TopWinningAgent.Business.Models;

namespace TopWinningAgent.Business.Services
{
    public class MailServiceImp : MailService
    {
        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly DateRangeService dateRangeService;
        private readonly MailStationService mailStationService;

        private readonly AppConfig appConfig;

        public MailServiceImp(
            FileService fileService,
            ExcelService excelService,
            ApplicationConfigurationService appConfigService,
            DateRangeService dateRangeService,
            MailStationService mailStationService)
        {
            this.fileService = fileService;
            this.excelService = excelService;
            this.dateRangeService = dateRangeService;
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
                Subject = $"{appConfig.Subject} {dateRangeService.FromDate(1): MM/dd/yyyy} to {dateRangeService.ToDate():MM/dd/yyyy}",
                Title = appConfig.Title,
                Body = fileService.ReadContent(),
                Attachments = attachments,
            };

            return mail;
        }

        public void SendMailWhenDataIsNotReady()
        {
            if (appConfig.FunctionIdNodata > 0)
            {
                var mail = new Mail
                {
                    Subject = $"{appConfig.Subject} {dateRangeService.FromDate(1): MM/dd/yyyy} to {dateRangeService.ToDate():MM/dd/yyyy}",
                    Title = appConfig.Title,
                    Body = fileService.ReadContentWhenDataIsNotReady(),
                    Attachments = new List<MailAttachment>(),
                    MailingListId = appConfig.FunctionIdNodata
                };

                mailStationService.Send(mail);
            }
        }
    }
}