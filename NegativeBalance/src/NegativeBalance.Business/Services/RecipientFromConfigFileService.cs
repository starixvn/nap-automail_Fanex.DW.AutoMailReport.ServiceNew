using System.Globalization;
using Fanex.MailDeliver.Model;
using Fanex.Service.Core;
using NegativeBalance.Business.Models;

namespace NegativeBalance.Business.Services
{
    public class RecipientFromConfigFileService : RecipientService
    {
        private readonly ApplicationConfigurationService appConfigService;

        public RecipientFromConfigFileService(ApplicationConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        public MailRecord CreateMail(Site site, string content)
        {
            var appConfig = appConfigService.Configurations;
            var siteName = site.LicenseeName.ToUpper(CultureInfo.CurrentCulture);
            return new MailRecord
            {
                Body = content,
                MailTo = appConfig.MailTo,
                MailCc = appConfig.MailCc,
                MailBcc = appConfig.MailBcc,
                Subject = appConfig.Subject.Replace("{0}", siteName).Replace("{1}", $"{SystemTime.Now:dd/MM/yyyy hh:mm:ss tt}"),
                Title = appConfig.Title
            };
        }
    }
}