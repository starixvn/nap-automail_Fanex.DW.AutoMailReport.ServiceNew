using System;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DateRangeServiceProviderUnitTests")]
    public class DateRangeServiceProviderUnitTests
    {
        private ApplicationConfigurationService appConfig;

        [SetUp]
        public void Initialize()
        {
            appConfig = Substitute.For<ApplicationConfigurationService>();
        }

        [Test]
        public void DateRange_Daily_ReturnRightObject()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 11, 01));
            var expected = Substitute.For<DateRangeService>();
            appConfig.IsManual.Returns(false);
            expected.DateStart.Returns(new DateTime(2017, 10, 01));
            expected.DateEnd.Returns(new DateTime(2017, 10, 31));

            // Act
            var result = new DateRangeServiceImp(appConfig);

            // Assert
            Assert.That(result.DateStart, Is.EqualTo(expected.DateStart));
            Assert.That(result.DateEnd, Is.EqualTo(expected.DateEnd));
        }

        [Test]
        public void DateRange_Manual_ReturnRightObject()
        {
            // Arrange
            var expected = Substitute.For<DateRangeService>();
            appConfig.IsManual.Returns(true);
            expected.DateStart.Returns(new DateTime(2017, 04, 01));
            expected.DateEnd.Returns(new DateTime(2017, 04, 30));

            // Act
            var result = new DateRangeServiceImp(appConfig);

            // Assert
            Assert.That(result.DateStart, Is.EqualTo(expected.DateStart));
            Assert.That(result.DateEnd, Is.EqualTo(expected.DateEnd));
        }
    }
}