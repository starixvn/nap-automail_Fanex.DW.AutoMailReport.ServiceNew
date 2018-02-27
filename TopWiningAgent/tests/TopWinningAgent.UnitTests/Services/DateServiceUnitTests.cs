using System;
using Fanex.Service.Core;
using NUnit.Framework;
using TopWinningAgent.Business.Services;

namespace TopWinningAgent.UnitTests.Services
{
    [Category("TopWinningAgent.UnitTests.Services.DateServiceUnitTests")]
    public class DateServiceUnitTests
    {
        [Test]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        public void GetFromDateLastWeek_Always_ReturnRightDate(int day)
        {
            // Arrange
            SystemTime.Set(new DateTime(2018, 02, day));
            var date = SystemTime.Now;
            var fromDateLastWeekResult = new DateTime(2018, 02, 05);

            // Action
            var service = new DateRangeServiceImp();
            var fromDateLastWeek = service.FromDate(1);

            // Assert
            Assert.That(fromDateLastWeek, Is.EqualTo(fromDateLastWeekResult));         
        }

        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        public void GetFromDateLast4Week_Always_ReturnRightDate(int day)
        {
            // Arrange
            SystemTime.Set(new DateTime(2018, 02, day));
            var date = SystemTime.Now;

            var fromDateLastWeekResult = new DateTime(2018, 01, 15);            

            // Action
            var service = new DateRangeServiceImp();
            var fromDateLastWeek = service.FromDate(4);            

            // Assert
            Assert.That(fromDateLastWeek, Is.EqualTo(fromDateLastWeekResult));          
        }

        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        public void GetFromDateLast12Week_Always_ReturnRightDate(int day)
        {
            // Arrange
            SystemTime.Set(new DateTime(2018, 02, day));
            var date = SystemTime.Now;

            var fromDateLastWeekResult = new DateTime(2017, 11, 20);

            // Action
            var service = new DateRangeServiceImp();
            var fromDateLastWeek = service.FromDate(12);

            // Assert
            Assert.That(fromDateLastWeek, Is.EqualTo(fromDateLastWeekResult));
        }

        [Test]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        public void GetToDateLastWeek_Always_ReturnRightDate(int day)
        {
            // Arrange
            SystemTime.Set(new DateTime(2018, 02, day));
            var date = SystemTime.Now;
            var toDateLastWeekResult = new DateTime(2018, 02, 11);

            // Action
            var service = new DateRangeServiceImp();
            var toDateLastWeek = service.ToDate();

            // Assert
            Assert.That(toDateLastWeek, Is.EqualTo(toDateLastWeekResult));
        }       
    }
}