using System;
using System.Collections.Generic;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using MaxLossMonitoring.Business.Models;

namespace MaxLossMonitoring.Business.Services
{
    public interface MailServiceLossMonitoringService
    {
        Mail Execute(Tuple<List<MonitorList>, List<MonitorListRaw>> dataSource);
    }

    public class MailServiceLossMonitoringServiceImp : MailServiceLossMonitoringService
    {
        private readonly string upwardArrowUrl = @"Templates/Images/UpwardArrow.png";
        private readonly string downwardArrowUrl = @"Templates/Images/DownardArrow.png";
        private readonly string horizontalLineUrl = @"Templates/Images/HorizontalLine.png";

        private readonly FileService fileService;
        private readonly ExcelService excelService;
        private readonly MailStationService mailStationService;

        private readonly AppConfig appConfig;

        public MailServiceLossMonitoringServiceImp(FileService fileService, ExcelService excelService, ApplicationConfigurationService appConfigService, MailStationService mailStationService)
        {
            this.fileService = fileService;
            this.excelService = excelService;
            this.mailStationService = mailStationService;

            appConfig = appConfigService.Configurations;
        }

        public Mail Execute(Tuple<List<MonitorList>, List<MonitorListRaw>> dataSource)
        {
            var fileName = excelService.Export(dataSource).Save();
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = string.Format(appConfig.Subject, $"{SystemTime.Now:yyyy/MM/dd}"),
                Title = appConfig.Title,
                Body = fileService.ReadContentWithAttachedFile(dataSource),
                Attachments = attachments,
                AlternativeViews = new List<MailLinkedResource>
                {
                    new MailLinkedResource(upwardArrowUrl, FileExtension.IMG_PNG, "lruworradrawpu"),
                    new MailLinkedResource(downwardArrowUrl, FileExtension.IMG_PNG, "lruworradrawnwod"),
                    new MailLinkedResource(horizontalLineUrl, FileExtension.IMG_PNG, "lruworradrawenillatnoziroh"),
                }
            };
            
            SendMailWithoutAttachedFile(new FileInfo(fileName).Name);

            return mail;
        }

        private void SendMailWithoutAttachedFile(string fileName)
        {
            var mail = new Mail
            {
                Subject = string.Format(appConfig.Subject, $"{SystemTime.Now:yyyy/MM/dd}"),
                Title = appConfig.Title,
                Body = fileService.ReadContentWithoutAttachedFile(fileName),
                Attachments = new List<MailAttachment>(),
                AlternativeViews = new List<MailLinkedResource>(),
                MailingListId = appConfig.FunctionId
            };

            if (appConfig.FunctionId > 0)
            {
                mailStationService.Send(mail);
            }
        }
    }
}