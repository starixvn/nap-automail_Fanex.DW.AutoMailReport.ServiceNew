using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsDaily.UnitTest.Service
{
    [Category("BetradarVirtualSportsDaily.UnitTest.Service DateServiceUnitTests")]
    public class DateServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void Date_SetManualIsFalse_ReturnRightValue()
        {
            // Arrange
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;

            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.Date;

            // Assert
            Assert.That(result, Is.EqualTo(result));
        }

        [Test]
        public void Date_SetManualIsTrue_ReturnRightValue()
        {
            // Arrange
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = true;
            SystemTime.Set(new DateTime(2017, 01, 10));

            var expected = new DateTime(2017, 01, 09);

            // Act
            var service = new DateServiceImp(appConfigService);
            var result = service.Date;

            // Assert
            Assert.That(result, Is.EqualTo(result));
        }
    }
}