using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ViewOfDataDetailServiceUnitTests")]
    public class ViewOfDataDetailServiceUnitTests
    {
        [Test]
        public void PeriodId_Always_Return1()
        {
            // Arrange
            var expected = 1;

            // Act
            var service = new ViewOfDataByDetailServiceImp(2);
            var result = service.PeriodId;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void PeriodId_SetAValue_ReturnValueHasSet()
        {
            // Arrange
            var expected = 2;

            // Act
            var service = new ViewOfDataByDetailServiceImp(2);            
            var result = service.SportId;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}