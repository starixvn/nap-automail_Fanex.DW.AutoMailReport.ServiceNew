using BettypeWith3rdProducts.Business.Repositories;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DWStatusByCubeCriteriaUnitTests")]
    public class DWStatusByCubeCriteriaUnitTests
    {
        [Test]
        public void Cube_Always_ReturnRightValue()
        {
            // Arrange
            var expected = "DW";

            // Act
            var result = new DWStatusByCubeCriteria().Cube;

            // Assert
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void GetSettingKey_Always_ReturnRightValue()
        {
            // Arrange
            var expected = "BI_GetDWStatusByCube";

            // Act
            var result = new DWStatusByCubeCriteria().GetSettingKey();

            // Assert
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void IsValid_Always_ReturnRightTrue()
        {
            // Arrange
            var expected = true;

            // Act
            var result = new DWStatusByCubeCriteria().IsValid();

            // Assert
            Assert.That(result.Equals(expected));
        }
    }
}