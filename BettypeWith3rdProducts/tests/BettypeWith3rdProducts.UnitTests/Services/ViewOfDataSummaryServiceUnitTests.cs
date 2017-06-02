using System;
using System.Linq;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ViewOfDataSummaryServiceUnitTests")]
    public class ViewOfDataSummaryServiceUnitTests
    {
        [Test]
        public void PeriodId_Always_ReturnNull()
        {
            // Arrange
            int? expected = null;

            // Act
            var service = new ViewOfDataBySummaryServiceImp();
            var result = service.PeriodId;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SportId_Always_ReturnNull()
        {
            // Arrange
            int? expected = null;

            // Act
            var service = new ViewOfDataBySummaryServiceImp();
            service.SportId = 5;
            var result = service.SportId;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }        
    }
}