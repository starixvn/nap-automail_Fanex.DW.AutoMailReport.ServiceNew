using System.Collections.Generic;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Services;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services DateRangeServiceUnitTests")]
    public class DateRangeServiceUnitTests
    {
        [Test]
        public void DateRange_Weekly_CallRightObject()
        {
            // Arrange
            var reportDateRange = ReportDateRange.Weekly;
            var expected = typeof(DateRangeByWeeklyImp);

            // Act
            var service = new DateRangeServiceImp();
            var result = service.DateRange(reportDateRange).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateRange_None_CallRightObject()
        {
            // Arrange
            var reportDateRange = ReportDateRange.None;
            var expected = typeof(DateRangeByWeeklyImp);

            // Act
            var service = new DateRangeServiceImp();
            var result = service.DateRange(reportDateRange).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateRange_Manual_CallRightObject()
        {
            // Arrange
            var reportDateRange = ReportDateRange.Manual;
            var expected = typeof(DateRangeByManualImp);

            // Act
            var service = new DateRangeServiceImp();
            var result = service.DateRange(reportDateRange).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}