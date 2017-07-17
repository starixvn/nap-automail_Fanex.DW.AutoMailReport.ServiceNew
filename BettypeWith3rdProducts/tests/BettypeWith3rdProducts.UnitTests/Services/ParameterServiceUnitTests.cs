using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ParameterServiceUnitTests")]
    public class ParameterServiceUnitTests
    {
        [Test]
        public void GetParameter_All_ReturnRightValue()
        {
            // Arrange
            var viewOfData = ViewOfData.All;
            var sportId = 1;
            var expected = new Parameter { SportId = null, PeriodId = 1 };

            // Act
            var service = new ParameterServiceImp();
            var result = service.GetParameter(viewOfData, sportId);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void GetParameter_Summary_ReturnRightValue()
        {
            // Arrange
            var viewOfData = ViewOfData.Summary;
            var sportId = 1;
            var expected = new Parameter { SportId = null, PeriodId = null };

            // Act
            var service = new ParameterServiceImp();
            var result = service.GetParameter(viewOfData, sportId);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void GetParameter_Detail_ReturnRightValue()
        {
            // Arrange
            var viewOfData = ViewOfData.Detail;
            var sportId = 1;
            var expected = new Parameter { SportId = 1, PeriodId = 1 };

            // Act
            var service = new ParameterServiceImp();
            var result = service.GetParameter(viewOfData, sportId);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}