using System.Collections.Generic;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using Fanex.UpdateExchangeRate.Business.Models;

namespace Fanex.UpdateExchangeRate.Business.Services
{
    public interface MailService
    {
        Mail Execute(List<CurrencyRate> data);
    }

    public class MailServiceImp : MailService
    {
        private readonly AppConfig appConfig;
        private readonly FileService fileService;
        private readonly ExcelService excelService;

        public MailServiceImp(ApplicationConfigurationService appConfigService, FileService fileService, ExcelService excelService)
        {
            appConfig = appConfigService.Configurations;
            this.fileService = fileService;
            this.excelService = excelService;
        }

        public Mail Execute(List<CurrencyRate> data)
        {
            var numberOfUpdates = data.Count;
            var fileName = excelService.Export(data).Save();
            var fileInfo = new FileInfo(fileName);
            var shortName = fileInfo.Name;
            
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = $"{appConfig.Subject}",
                Title = appConfig.Title,
                Body = fileService.ReadContent(shortName, numberOfUpdates),
                Attachments = attachments,
            };

            return mail;
        }        
    }
}