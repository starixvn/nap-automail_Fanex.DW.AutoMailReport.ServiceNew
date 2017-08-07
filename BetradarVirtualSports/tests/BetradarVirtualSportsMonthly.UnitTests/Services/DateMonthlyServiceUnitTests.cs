using System;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services DateMonthlyServiceUnitTests")]
    public class DateMonthlyServiceUnitTests
    {
        [Test]
        public void FromDate_FirstDayOfYear_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var expected = new DateTime(2016, 12, 01);

            // Act
            var service = new DateMonthlyService();
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FromDate_AnyDayOfYear_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 06, 03));
            var expected = new DateTime(2017, 05, 01);

            // Act
            var service = new DateMonthlyService();
            var result = service.FromDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void To_FirstDayOfYear_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var expected = new DateTime(2016, 12, 31);

            // Act
            var service = new DateMonthlyService();
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToDate_AnyDayOfYear_ReturnRightValue()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 06, 03));
            var expected = new DateTime(2017, 05, 31);

            // Act
            var service = new DateMonthlyService();
            var result = service.ToDate;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}