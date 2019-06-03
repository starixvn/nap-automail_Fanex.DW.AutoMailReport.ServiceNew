using System;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.DailyBetBuilderCriteriaServiceUnitTests")]
    public class DailyBetBuilderCriteriaServiceUnitTests
    {
        [Test]
        public void WinlostDate_IsManualTrue_ReturnRightDate()
        {
            // Arrange
            AppConfigService.AppConfig = new ConfigSettingModel
            {
                WinlostDate = new DateTime(2019, 05, 01),
                IsManual = true
            };

            var expected = new DateTime(2019, 05, 01);

            // Act
            var service = new CriteriaServiceImp();
            var result = service.WinlostDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void WinlostDate_IsManualFalse_ReturnRightDate()
        {
            // Arrange
            AppConfigService.AppConfig = new ConfigSettingModel
            {
                IsManual = false
            };

            SystemTime.Set(new DateTime(2019, 05, 02));
            var expected = new DateTime(2019, 05, 01);

            // Act
            var service = new CriteriaServiceImp();
            var result = service.WinlostDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}