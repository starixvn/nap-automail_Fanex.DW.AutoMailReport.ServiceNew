using Fanex.MailDeliver.Model;
using Fanex.Service.Core;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services RecipientServiceUnitTests")]
    public class RecipientServiceUnitTests
    {
        [Test]
        public void CreateMail_SendMailFromConfig_ReturnRightContent()
        {
            // Arrange
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            var appConfig = CreateAppConfig(true);
            SystemTime.Set(new System.DateTime(2017, 01, 01, 01, 01, 01));
            appConfigService.Configurations.Returns(appConfig);
            var site = new Site { ToList = "mail.from.database@a.com", CcList = string.Empty, BccList = string.Empty, LicenseeName = "12bet", SiteId = 1, IsAvailable = true };
            var content = "body";
            var expected = TestLoader.DataLoader<MailRecord>("MailRecordFromConfig-expected.json");

            // Act
            var service = new RecipientServiceImp(appConfigService);
            var result = service.CreateMail(site, content);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void CreateMail_SendMailFromDatabase_ReturnRightContent()
        {
            // Arrange
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            var appConfig = CreateAppConfig(false);
            SystemTime.Set(new System.DateTime(2017, 01, 01, 01, 01, 01));
            appConfigService.Configurations.Returns(appConfig);
            var site = new Site { ToList = "mail.from.database@a.com", CcList = string.Empty, BccList = string.Empty, LicenseeName = "bodog", SiteId = 1, IsAvailable = true };
            var content = "body";
            var expected = TestLoader.DataLoader<MailRecord>("MailRecordFromDatabase-expected.json");

            // Act
            var service = new RecipientServiceImp(appConfigService);
            var result = service.CreateMail(site, content);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private AppConfig CreateAppConfig(bool isSendMailFromConfig)
        {
            return new AppConfig
            {
                MailTo = "mail.from.config.file@a.com",
                MailCc = string.Empty,
                MailBcc = string.Empty,
                IsSendMailFromConfig = isSendMailFromConfig,
                Subject = "Automail Report - {0} - List of Members with Negative Balance By Match - {1}",
                Title = "title"
            };
        }
    }
}