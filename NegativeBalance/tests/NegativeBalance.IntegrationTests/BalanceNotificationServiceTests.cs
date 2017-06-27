using NegativeBalance.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace NegativeBalance.IntegrationTests
{
    [Category("NegativeBalance.IntegrationTests BalanceNotificationServiceTests")]
    public class BalanceNotificationServiceTests
    {
        private Container container;

        [OneTimeSetUp]
        public void Initialize()
        {
            container = new Container();
            FanexDataRegisterService.ConfigureFanexData();
            NegativeBalanceInjector.Register(container);
        }

        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            Initialize();

            // Act
            var service = container.GetInstance<BalanceNotificationService>();
            service.Execute();
        }
    }
}