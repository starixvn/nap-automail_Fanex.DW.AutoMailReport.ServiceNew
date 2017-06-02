using System;
using BettypeWith3rdProducts.Business.Services;
using BettypeWith3rdProducts.Business.Utility;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DateRangeDailyServiceUnitTests")]
    public class DateRangeDailyServiceUnitTests
    {
        [Test]
        public void DateStart_TodayIsTheFirstDayOfATheYear_ReturnRightIndicateDate()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var expected = new DateTime(2016, 12, 01);

            // Act
            var service = new DateRangeDailyServiceImp();
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateEnd_TodayIsTheFirstDayOfATheYear_ReturnRightIndicateDate()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var expected = new DateTime(2016, 12, 31);

            // Act
            var service = new DateRangeDailyServiceImp();
            var result = service.DateEnd;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateStart_Always_ReturnRightIndicateDate()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 15));
            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new DateRangeDailyServiceImp();
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateEnd_Always_ReturnRightIndicateDate()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 15));
            var expected = new DateTime(2017, 01, 14);

            // Act
            var service = new DateRangeDailyServiceImp();
            var result = service.DateEnd;

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