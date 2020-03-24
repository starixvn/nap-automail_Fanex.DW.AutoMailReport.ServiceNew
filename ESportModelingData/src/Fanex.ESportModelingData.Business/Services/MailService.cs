using System;
using System.Collections.Generic;
using Fanex.MailDelivery.Core.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface MailService
    {
        Mail CreateMail(DateTime dateStart, DateTime dateEnd, List<string> fileNames);
    }

    public class MailServiceImp : MailService
    {
        private readonly MailContentService mailContentService;

        public MailServiceImp(MailContentService mailContentService)
        {
            this.mailContentService = mailContentService;
        }

        public Mail CreateMail(DateTime dateStart, DateTime dateEnd, List<string> fileNames)
        {
            var mail = new Mail
            {
                Subject = $"E-Sports {AppConfigService.AppConfig.Period} Modeling Data From {dateStart:dd-MM-yyyy} To {dateEnd:dd-MM-yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = mailContentService.GenerateMailContent(fileNames),
                Attachments = new List<MailAttachment>()
            };

            return mail;
        }
    }
}