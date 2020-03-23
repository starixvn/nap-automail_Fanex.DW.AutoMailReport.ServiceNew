using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.ParamServiceUnitTests")]
    public class ParamServiceUnitTests
    {
        [Test]
        public void Dates_IsManualTrue_ReturnRightDateList()
        {
            // Arrange
            AppConfigService.AppConfig.Period = Period.Manual;
            AppConfigService.AppConfig.DateStart = new DateTime(2019, 06, 01);
            AppConfigService.AppConfig.DateEnd = new DateTime(2019, 06, 20);
            var dateTimeService = new DateTimeServiceImp();
            var expected = new List<DateTimeModel>
            {
                new DateTimeModel { DateStart = new DateTime(2019, 06, 03), DateEnd = new DateTime(2019, 06, 09) },
                new DateTimeModel { DateStart = new DateTime(2019, 06, 10), DateEnd = new DateTime(2019, 06, 16) }
            };

            // Act
            var service = new ParamServiceImp(dateTimeService);
            var result = service.Dates();

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }

        [Test]
        public void Dates_IsManualFalse_ReturnRightDateList()
        {
            // Arrange
            SystemTime.Set(new DateTime(2019, 06, 01));
            AppConfigService.AppConfig.Period = Period.Weekly;
            AppConfigService.AppConfig.DateStart = new DateTime(2019, 06, 01);
            AppConfigService.AppConfig.DateEnd = new DateTime(2019, 06, 10);
            var dateTimeService = new DateTimeServiceImp();
            var expected = new List<DateTimeModel>
            {
                new DateTimeModel { DateStart = new DateTime(2019, 06, 03), DateEnd = new DateTime(2019, 06, 09) }
            };

            // Act
            var service = new ParamServiceImp(dateTimeService);
            var result = service.Dates();

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(result));
        }
    }
}