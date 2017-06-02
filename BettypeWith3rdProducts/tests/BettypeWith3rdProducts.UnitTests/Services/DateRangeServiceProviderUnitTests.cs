using System;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DateRangeServiceProviderUnitTests")]
    public class DateRangeServiceProviderUnitTests
    {
        [Test]
        public void DateRange_Daily_ReturnRightObject()
        {
            // Arrange
            var dateRange = DateRange.Daily;
            var expected = typeof(DateRangeDailyServiceImp);

            // Act
            var service = new DateRangeServiceProviderImp();
            var result = service.Date(dateRange).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateRange_Manual_ReturnRightObject()
        {
            // Arrange
            var dateRange = DateRange.Manual;
            var expected = typeof(DateRangeManualServiceImp);

            // Act
            var service = new DateRangeServiceProviderImp();
            var result = service.Date(dateRange).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Daily", ExpectedResult = DateRange.Daily)]
        [TestCase("manual", ExpectedResult = DateRange.Manual)]
        [TestCase("AnyText", ExpectedResult = DateRange.None)]
        public DateRange Enum_Always_ReturnRightEnumType(string reportType)
        {
            // Arrange
            DateRange dateRange;
            Enum.TryParse(reportType, true, out dateRange);

            return dateRange;
        }
    }
}