using Fanex.ScoreBox.Business.Services;
using NUnit.Framework;

namespace Fanex.ScoreBox.UnitTests.Services
{
    [Category("Fanex.ScoreBox.UnitTests.Services.DateProviderUnitTests")]
    public class DateProviderUnitTests
    {
        [Test]
        public void Date_Daily_ReturnRightIntance()
        {
            // Arrange
            var expected = typeof(DateDailyServiceImp);

            // Act
            var service = new DateProviderImp();
            var result = service.Date(true);

            // Assert
            Assert.That(result.GetType().Equals(expected));
        }

        [Test]
        public void Date_Monthly_ReturnRightIntance()
        {
            // Arrange
            var expected = typeof(DateMonthlyServiceImp);

            // Act
            var service = new DateProviderImp();
            var result = service.Date(false);

            // Assert
            Assert.That(result.GetType().Equals(expected));
        }
    }
}