using FastMarketWeeklyPerformance.Business.Repositories;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Repositories
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Repositories FastMarketByWeeklyCriteriaUnitTests")]
    public class FastMarketByWeeklyCriteriaUnitTests
    {
        [Test]
        public void GetSettingKey_Always_ReturnRightKey()
        {
            // Arrange
            var expected = "Acc_Rpt_Automail_FastMarketByWeekly";

            // Act
            var service = new FastMarketByWeeklyCriteria();
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
            var service = new FastMarketByWeeklyCriteria();
            var result = service.IsValid();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}