using Fanex.ScoreBox.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;
using System;

namespace Fanex.ScoreBox.UnitTests.Services
{
    [Category("Fanex.ScoreBox.UnitTests.Services.DateDailyServiceUnitTests")]
    public class DateDailyServiceUnitTests
    {
        [Test]
        public void FromDate_Always_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2019, 08, 06));
            var expected = new DateTime(2019, 08, 05);

            // Act
            var service = new DateDailyServiceImp();
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_Always_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2019, 08, 06));
            var expected = new DateTime(2019, 08, 05);

            // Act
            var service = new DateDailyServiceImp();
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}