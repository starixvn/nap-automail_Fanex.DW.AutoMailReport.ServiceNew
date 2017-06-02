using System;
using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DateRangeManualServiceUnitTests")]
    public class DateRangeManualServiceUnitTests
    {
        [Test]
        public void DateStart_Always_ReturnRightIndicateDate()
        {
            // Arrange
            var expected = new DateTime(2017, 04, 01);

            // Act
            var service = new DateRangeManualServiceImp();
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateEnd_Always_ReturnRightIndicateDate()
        {
            // Arrange
            var expected = new DateTime(2017, 04, 30);

            // Act
            var service = new DateRangeManualServiceImp();
            var result = service.DateEnd;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}