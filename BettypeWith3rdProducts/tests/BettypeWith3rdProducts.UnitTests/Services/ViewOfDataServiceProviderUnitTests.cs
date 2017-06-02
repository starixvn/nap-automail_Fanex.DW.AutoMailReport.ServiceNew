using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ViewOfDataServiceProviderUnitTests")]
    public class ViewOfDataServiceProviderUnitTests
    {
        [Test]
        public void ViewOfDataByAllService_Always_ReturnRightObjectType()
        {
            // Arrange
            var expected = typeof(ViewOfDataByAllServiceImp);

            // Act
            var service = new ViewOfDataServiceProviderImp();
            var result = service.View(ViewOfData.All, 2).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ViewOfDataByDetailService_Always_ReturnRightObjectType()
        {
            // Arrange
            var expected = typeof(ViewOfDataByDetailServiceImp);

            // Act
            var service = new ViewOfDataServiceProviderImp();
            var result = service.View(ViewOfData.Detail, 2).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ViewOfDataBySummaryService_Always_ReturnRightObjectType()
        {
            // Arrange
            var expected = typeof(ViewOfDataBySummaryServiceImp);

            // Act
            var service = new ViewOfDataServiceProviderImp();
            var result = service.View(ViewOfData.Summary, 2).GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}