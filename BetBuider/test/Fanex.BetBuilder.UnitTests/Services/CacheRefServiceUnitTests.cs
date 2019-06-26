using Fanex.BetBuilder.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.CacheRefServiceUnitTests")]
    public class CacheRefServiceUnitTests
    {
        private Container container;

        [SetUp]
        public void Init()
        {
            container = new Container();            
            RegisterService.RegisterContainer(container);
        }

        [Test]
        public void GetLeagueName_Always_ReturnRightValue()
        {
            // Arrange
            var leagueId = 49426;
            var expected = "*ITALY SERIE A - WHICH TEAM TO KICK OFF";
            var service = container.GetInstance<CacheRefService>();

            // Act
            var result = service.GetLeagueName(leagueId.ToString());

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}