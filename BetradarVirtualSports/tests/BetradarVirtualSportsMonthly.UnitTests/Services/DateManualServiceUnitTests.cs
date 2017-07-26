using System;
using System.IO;
using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services DateManualServiceUnitTests")]
    public class DateManualServiceUnitTests
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
        public void FromDate_Always_ReturnRightData()
        {
            // Arrange
            var expected = new DateTime(2017, 07, 01);

            // Act
            var service = new DateManualService(appConfigService);
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_Always_ReturnRightData()
        {
            // Arrange
            var expected = new DateTime(2017, 07, 31);

            // Act
            var service = new DateManualService(appConfigService);
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}