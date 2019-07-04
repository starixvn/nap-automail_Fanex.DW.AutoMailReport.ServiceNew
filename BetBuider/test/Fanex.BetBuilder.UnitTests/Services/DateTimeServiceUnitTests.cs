using System;
using Fanex.BetBuilder.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.DateTimeServiceUnitTests")]
    public class DateTimeServiceUnitTests
    {
        [Test]
        public void FromDate_IsManualTrue_ReturnRightDate()
        {
            // Arrange
            AppConfigService.AppConfig.IsManual = true;
            AppConfigService.AppConfig.WinlostDate = new DateTime(2019, 05, 01);
            var expected = new DateTime(2019, 04, 01);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.FromDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FromDate_IsManualFalse_ReturnRightDate()
        {
            // Arrange
            AppConfigService.AppConfig.IsManual = false;
            AppConfigService.AppConfig.WinlostDate = new DateTime(2019, 06, 12);

            var expected = new DateTime(2019, 06, 01);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.FromDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_IsManualTrue_ReturnRightDate()
        {
            // Arrange
            AppConfigService.AppConfig.IsManual = true;
            AppConfigService.AppConfig.WinlostDate = new DateTime(2019, 05, 01);
            var expected = new DateTime(2019, 04, 30);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.ToDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_IsManualFalse_ReturnRightDate()
        {
            // Arrange
            AppConfigService.AppConfig.IsManual = false;
            AppConfigService.AppConfig.WinlostDate = new DateTime(2019, 06, 12);

            var expected = new DateTime(2019, 06, 30);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.ToDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TearDown]
        public void Reset()
        {
            AppConfigService.AppConfig.IsManual = true;
            AppConfigService.AppConfig.WinlostDate = new DateTime(2019, 05, 01);
            SystemTime.Reset();
        }
    }
}