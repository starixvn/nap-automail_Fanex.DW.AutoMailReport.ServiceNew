using System;
using System.Linq;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ViewOfDataAllServiceUnitTests")]
    public class ViewOfDataAllServiceUnitTests
    {
        [Test]
        public void PeriodId_Always_Return1()
        {
            // Arrange
            var expected = 1;

            // Act
            var service = new ViewOfDataByAllServiceImp();
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
            var service = new ViewOfDataByAllServiceImp();
            var result = service.SportId;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }        
    }
}