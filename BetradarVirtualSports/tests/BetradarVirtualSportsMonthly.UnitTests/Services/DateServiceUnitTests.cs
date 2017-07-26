using System;
using System.IO;
using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services DateServiceUnitTests")]
    public class DateServiceUnitTests
    {
        private ApplicationConfigurationService appConfigService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
        }

        [Test]
        public void FromDate_SetManualIsTrue_ReturnRightValue()
        {
            // Arrange
            appConfigService.Configurations.IsManual = true;
            var expected = new DateTime(2017, 07, 01);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_SetManualIsTrue_ReturnRightValue()
        {
            // Arrange
            appConfigService.Configurations.IsManual = true;
            var expected = new DateTime(2017, 07, 31);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FromDate_SetManualIsFalse_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 06, 03));
            appConfigService.Configurations.IsManual = false;
            var expected = new DateTime(2017, 05, 01);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_SetManualIsFalse_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 06, 03));
            appConfigService.Configurations.IsManual = false;
            var expected = new DateTime(2017, 05, 31);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}