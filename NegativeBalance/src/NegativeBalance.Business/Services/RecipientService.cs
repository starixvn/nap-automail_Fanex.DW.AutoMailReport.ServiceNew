using System.Collections.Generic;
using Fanex.MailDeliver.Model;
using NegativeBalance.Business.Models;

namespace NegativeBalance.Business.Services
{
    public interface RecipientService
    {
        MailRecord CreateMail(Site site, string content);
    }

    public class RecipientServiceImp : RecipientService
    {
        private readonly ApplicationConfigurationService appConfigService;

        public RecipientServiceImp(ApplicationConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        private Dictionary<bool, RecipientService> Recipient => new Dictionary<bool, RecipientService>
        {
            { true, new RecipientFromConfigFileService(appConfigService) },
            { false, new RecipientFromDatabaseService(appConfigService) }
        };

        private static Dictionary<string, string> LicenseesChangeName => new Dictionary<string, string>
        {
            { "bodog", "Haydock" },
            { "avia", "Oriental Game" },
            { "ibo", "Kzing" },
        };

        public MailRecord CreateMail(Site site, string content)
        {
            var licenseeName = site.LicenseeName.ToLowerInvariant();
            site.LicenseeName = LicenseesChangeName.ContainsKey(licenseeName)
                ? LicenseesChangeName[licenseeName]
                : site.LicenseeName;

            return Recipient[appConfigService.Configurations.IsSendMailFromConfig].CreateMail(site, content);
        }
    }
}