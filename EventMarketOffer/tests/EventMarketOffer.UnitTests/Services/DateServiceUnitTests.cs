using System;
using EventMarketOffer.Business.Models;
using EventMarketOffer.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace EventMarketOffer.UnitTests.Services
{
    [Category("EventMarketOffer.UnitTests.Services DateServiceUnitTests")]
    public class DateServiceUnitTests
    {
        private AppConfigurationService appConfigService;

        [SetUp]
        public void Initialize()
        {
            SystemTime.Set(new DateTime(2017, 08, 08));
            appConfigService = Substitute.For<AppConfigurationService>();
            appConfigService.Configurations.Returns(new AppConfig
            {
                DateStart = new DateTime(2017, 01, 01),
                DateEnd = new DateTime(2017, 01, 31),
                IsManual = true
            });
        }

        [Test]
        public void DateStart_SetIsManualTrue_ReturnRightDate()
        {
            // Arrange
            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateEnd_SetIsManualTrue_ReturnRightDate()
        {
            // Arrange
            var expected = new DateTime(2017, 01, 31);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.DateEnd;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateStart_SetIsManualFalse_ReturnRightDate()
        {
            // Arrange
            appConfigService.Configurations.IsManual = false;
            var expected = new DateTime(2017, 07, 01);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateEnd_SetIsManualFalse_ReturnRightDate()
        {
            // Arrange
            appConfigService.Configurations.IsManual = false;
            var expected = new DateTime(2017, 07, 31);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.DateEnd;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}