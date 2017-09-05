using System;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;
using TOControl.CustomerMovement.Business.Models;
using TOControl.CustomerMovement.Business.Services;

namespace TOControl.CustomerMovement.UnitTests.Services
{
    [Category("TOControl.CustomerMovement.UnitTests.Services DateServiceUnitTests")]
    public class DateServiceUnitTests
    {
        [Test]
        public void ToDate_Always_ReturnYesterday()
        {
            // Arrange
            var appConfig = Substitute.For<ApplicationConfigurationService>();
            SystemTime.Set(new DateTime(2017, 07, 25));
            var expected = new DateTime(2017, 07, 24);

            // Act
            var service = new DateServiceImp(appConfig);
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FromDate_Always_ReturnRightDate()
        {
            // Arrange
            var appConfig = Substitute.For<ApplicationConfigurationService>();
            appConfig.Configurations.Returns(new AppConfig { PeriodDate = 2 });
            SystemTime.Set(new DateTime(2017, 07, 25));
            var expected = new DateTime(2017, 07, 22);

            // Act
            var service = new DateServiceImp(appConfig);
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}