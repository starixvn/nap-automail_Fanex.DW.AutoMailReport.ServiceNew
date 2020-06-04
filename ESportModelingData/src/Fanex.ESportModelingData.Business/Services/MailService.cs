using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface MailService
    {
        Mail CreateMail(List<string> fileNames);
    }

    public class MailServiceImp : MailService
    {
        private readonly MailContentService mailContentService;
        private readonly GenerateTemplateProvider template;

        public MailServiceImp(MailContentService mailContentService, GenerateTemplateProvider template)
        {
            this.mailContentService = mailContentService;
            this.template = template;
        }

        public Mail CreateMail(List<string> fileNames)
        {
            var period = AppConfigService.AppConfig.Period;
            var sport = AppConfigService.AppConfig.Sport;
            var subject = template.Execute(period, DateTime.Today).Subject(sport, period);

            var mail = new Mail
            {
                Subject = subject,
                Title = "ONEworks Auto Generated Report Administrator",
                Body = mailContentService.GenerateMailContent(fileNames),
                Attachments = new List<MailAttachment>()
            };

            return mail;
        }
    }
}