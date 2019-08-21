using Fanex.ScoreBox.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;
using System;

namespace Fanex.ScoreBox.UnitTests.Services
{
    [Category("Fanex.ScoreBox.UnitTests.Services.DateMonthlyServiceUnitTests")]
    public class DateMonthlyServiceUnitTests
    {
        [Test]
        public void FromDate_Always_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2019, 08, 06));
            var expected = new DateTime(2019, 07, 01);

            // Act
            var service = new DateMonthlyServiceImp();
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_Always_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2019, 08, 06));
            var expected = new DateTime(2019, 07, 31);

            // Act
            var service = new DateMonthlyServiceImp();
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}