using System;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Services;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services DateRangeByWeeklyUnitTests")]
    public class DateRangeByWeeklyUnitTests
    {
        [Test]
        public void FromDate_Always_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 09));
            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new DateRangeByWeeklyImp();
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_Always_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 09));
            var expected = new DateTime(2017, 01, 08);

            // Act
            var service = new DateRangeByWeeklyImp();
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TearDown]
        public void Reset()
        {
            SystemTime.Reset();
        }
    }
}