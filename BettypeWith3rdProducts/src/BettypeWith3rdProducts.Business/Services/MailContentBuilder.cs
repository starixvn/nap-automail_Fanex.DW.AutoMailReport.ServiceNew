using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface MailContentBuilder
    {
        Mail ReadContentWithAttachment(string fileName);

        Mail ReadContentWithoutAttachment(string fileName);
    }

    public class MailContentBuilderImp : MailContentBuilder
    {
        private readonly ApplicationConfigurationService appConfig;
        private readonly FileService fileService;

        public MailContentBuilderImp(ApplicationConfigurationService appConfig, FileService fileService)
        {
            this.appConfig = appConfig;
            this.fileService = fileService;
        }

        public Mail ReadContentWithAttachment(string fileName)
        {
            var shortFileNames = new FileInfo(fileName).Name;
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} - {DateTime.Now:yyyy/MM/dd}",
                Title = appConfig.Title,
                Body = fileService.ReadContentWhenDataIsAvailable(shortFileNames, "ContentWithAttachment.cshtml"),
                Attachments = attachments,
            };

            return mail;
        }

        public Mail ReadContentWithoutAttachment(string fileName)
        {
            var shortFileNames = new FileInfo(fileName).Name;
            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} - {DateTime.Now:yyyy/MM/dd}",
                Title = appConfig.Title,
                Body = fileService.ReadContentWhenDataIsAvailable(shortFileNames, "ContentWithoutAttachment.cshtml"),
                Attachments = new List<MailAttachment>(),
                MailingListId = appConfig.FunctionIdWithoutAttachment
            };

            return mail;
        }
    }
}