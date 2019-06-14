using System;
using System.Collections.Generic;
using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.DateTimeServiceUnitTests")]
    public class DateTimeServiceUnitTests
    {
        [Test]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        public void DateStart_IsManualFalse_ReturnRightValue(int day)
        {
            // Arrange
            AppConfigService.AppConfig = new ConfigSettingModel();
            SystemTime.Set(new DateTime(2019, 06, day));
            var expected = new DateTime(2019, 06, 03);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.DateStart();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        }

        [Test]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        public void DateEnd_IsManualFalse_ReturnRightValue(int day)
        {
            // Arrange
            AppConfigService.AppConfig = new ConfigSettingModel();
            SystemTime.Set(new DateTime(2019, 06, day));
            var expected = new DateTime(2019, 06, 09);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.DateEnd();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        }

        [Test]
        public void DateStart_IsManualTrue_ReturnRightValue()
        {
            // Arrange
            AppConfigService.AppConfig = new ConfigSettingModel { IsManual = true, DateStart = new DateTime(2019, 06, 10) };
            var expected = new DateTime(2019, 06, 10);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.DateStart();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        }

        [Test]
        public void DateEnd_IsManualTrue_ReturnRightValue()
        {
            // Arrange
            AppConfigService.AppConfig = new ConfigSettingModel { IsManual = true, DateStart = new DateTime(2019, 06, 10) };
            var expected = new DateTime(2019, 06, 16);

            // Act
            var service = new DateTimeServiceImp();
            var result = service.DateEnd();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        }

        [Test]
        public void DateStart_IsManualTrue_History()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            AppConfigService.AppConfig = new ConfigSettingModel { IsManual = true, DateStart = new DateTime(2017, 01, 01) };
            var expected = new DateTime(2017, 01, 02);

            // Act
            var service = new DateTimeServiceImp();

            var result = service.DateStart();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        }

        [Test]
        public void DateEnd_IsManualTrue_History()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            AppConfigService.AppConfig = new ConfigSettingModel { IsManual = true, DateStart = new DateTime(2017, 01, 01) };
            var expected = new DateTime(2017, 01, 08);

            // Act
            var service = new DateTimeServiceImp();

            var result = service.DateEnd();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        }

        [TearDown]
        public void Reset()
        {
            SystemTime.Reset();
        }
    }
}