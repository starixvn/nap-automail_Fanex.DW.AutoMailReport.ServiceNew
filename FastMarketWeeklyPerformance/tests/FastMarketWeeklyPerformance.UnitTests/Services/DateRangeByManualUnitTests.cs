using System;
using FastMarketWeeklyPerformance.Business.Services;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services DateRangeByManualUnitTests")]
    public class DateRangeByManualUnitTests
    {
        [Test]
        public void FromDate_Always_ReturnRightValue()
        {
            // Arrange
            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new DateRangeByManualImp();
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_Always_ReturnRightValue()
        {
            // Arrange
            var expected = new DateTime(2017, 01, 07);

            // Act
            var service = new DateRangeByManualImp();
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}