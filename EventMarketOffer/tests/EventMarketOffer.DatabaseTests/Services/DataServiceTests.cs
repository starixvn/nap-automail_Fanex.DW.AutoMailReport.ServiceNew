using EventMarketOffer.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace EventMarketOffer.DatabaseTests.Services
{
    [Category("EventMarketOffer.DatabaseTests.Services DataServiceTests")]
    public class DataServiceTests
    {
        // [Test]
        public void Query_Always_ConnectToDBSuccessfully()
        {
            // Arrange
            var container = new Container();
            FanexDataRegisterService.ConfigureFanexData();
            SimpleInjectorService.Register(container);
            var expected = 7;

            // Act
            var service = container.GetInstance<DataService>();
            var result = service.Query();
            var columnCount = result.Columns.Count;

            // Assert
            Assert.That(expected, Is.EqualTo(columnCount));
        }
    }
}