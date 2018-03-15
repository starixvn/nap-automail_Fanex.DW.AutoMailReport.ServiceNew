using Fanex.UpdateExchangeRate.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace UpdateExchangeRate.IntegrationTests
{
    [Category("UpdateExchangeRate.IntegrationTests NotificationServiceTests")]
    public class NotificationServiceTests
    {
        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            FanexDataRegisterService.ConfigureFanexData();

            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            // Act
            var service = container.GetInstance<NotificationService>();
            var result = service.Execute();

            // Assert
            Assert.That(result != null);
        }
    }
}