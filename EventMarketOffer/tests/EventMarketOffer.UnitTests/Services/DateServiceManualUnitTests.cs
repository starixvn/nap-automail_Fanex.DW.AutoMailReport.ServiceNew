using System;
using EventMarketOffer.Business.Models;
using EventMarketOffer.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace EventMarketOffer.UnitTests.Services
{
    [Category("EventMarketOffer.UnitTests.Services DateServiceManualUnitTests")]
    public class DateServiceManualUnitTests
    {
        private AppConfigurationService appConfigService;

        [SetUp]
        public void Initialize()
        {
            appConfigService = Substitute.For<AppConfigurationService>();
            appConfigService.Configurations.Returns(new AppConfig
            {
                DateStart = new DateTime(2017, 07, 01),
                DateEnd = new DateTime(2017, 07, 31)
            });
        }

        [Test]
        public void DateStart_Always_ReturnRightDate()
        {
            // Arrange
            var expected = new DateTime(2017, 07, 01);

            // Act
            var service = new DateServiceManual(appConfigService);
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateEnd_Always_ReturnRightDate()
        {
            // Arrange
            var expected = new DateTime(2017, 07, 31);

            // Act
            var service = new DateServiceManual(appConfigService);
            var result = service.DateEnd;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}