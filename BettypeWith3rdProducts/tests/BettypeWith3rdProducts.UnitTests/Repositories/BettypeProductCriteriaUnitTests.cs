using BettypeWith3rdProducts.Business.Repositories;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Repositories
{
    [Category("BettypeWith3rdProducts.UnitTests.Repositories BettypeProductCriteriaUnitTests")]
    public class BettypeProductCriteriaUnitTests
    {
        [Test]
        public void IsValid_Always_ReturnTrue()
        {
            // Arrange

            // Act
            var service = new BettypeProductCriteria();
            var result = service.IsValid();

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }
    }
}