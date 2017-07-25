using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service DateManualServiceUnitTests")]
    public class DateManualServiceUnitTests
    {
        [Test]
        public void Date_Always_ReturnRightDate()
        {
            // Arrange
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            var expected = new DateTime(2017, 07, 01);

            // Act
            var service = new DateManualService(appConfigService);
            var result = service.Date;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}