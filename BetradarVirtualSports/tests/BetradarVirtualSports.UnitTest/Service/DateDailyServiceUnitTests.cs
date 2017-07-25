using System;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service DateDailyServiceUnitTests")]
    public class DateDailyServiceUnitTests
    {
        [Test]
        public void Date_Always_ReturnRightDate()
        {
            // Arrange
            var expected = new DateTime(2017, 01, 01);
            SystemTime.Set(new DateTime(2017, 01, 02));

            // Act
            var service = new DateDailyService();
            var result = service.Date;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}