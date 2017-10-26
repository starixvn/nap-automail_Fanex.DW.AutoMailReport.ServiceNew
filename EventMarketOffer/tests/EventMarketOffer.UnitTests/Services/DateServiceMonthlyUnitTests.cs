using System;
using EventMarketOffer.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace EventMarketOffer.UnitTests.Services
{
    [Category("EventMarketOffer.UnitTests.Services DateServiceMonthlyUnitTests")]
    public class DateServiceMonthlyUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            SystemTime.Set(new DateTime(2017, 08, 08));
        }

        [Test]
        public void DateEnd_Always_ReturnRightDate()
        {
            // Arrange            
            var expected = new DateTime(2017, 07, 31);

            // Act
            var service = new DateServiceMonthly();
            var result = service.DateEnd;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void DateStart_Always_ReturnRightDate()
        {
            // Arrange
            var expected = new DateTime(2017, 07, 01);

            // Act
            var service = new DateServiceMonthly();
            var result = service.DateStart;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}