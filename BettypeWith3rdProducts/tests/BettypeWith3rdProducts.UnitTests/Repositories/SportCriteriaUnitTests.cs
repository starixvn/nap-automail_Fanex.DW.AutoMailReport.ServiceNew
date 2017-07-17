using BettypeWith3rdProducts.Business.Repositories;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Repositories
{
    [Category("BettypeWith3rdProducts.UnitTests.Repositories SportCriteriaUnitTests")]
    public class SportCriteriaUnitTests
    {
        [Test]
        public void GetSettingKey_Always_ReturnRightTheKey()
        {
            // Arrange
            var expected = "Acc_Rpt_Automail_BettypeWithProducts_Sport_Select";

            // Act
            var service = new SportCriteria();
            var result = service.GetSettingKey();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsValid_Always_ReturnTrue()
        {
            // Arrange
            var expected = true;

            // Act
            var service = new SportCriteria();
            var result = service.IsValid();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}