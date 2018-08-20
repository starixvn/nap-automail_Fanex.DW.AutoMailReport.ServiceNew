using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NUnit.Framework;
using SimpleInjector;

namespace EventMarketOffer.IntegrationTests.Services
{
    [Category("EventMarketOffer.IntegrationTests.Services DistributedNotificationServiceTests")]
    public class DistributedNotificationServiceTests
    {
        // [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            FanexDataRegisterService.ConfigureFanexData();
            var container = new Container();
            SimpleInjectorService.Register(container);
            var expected = JobStatus.Success;

            // Act
            var service = container.GetInstance<DistributedNotificationService>();
            var result = service.Execute();

            // Assert
            Assert.That(result.JobStatus, Is.EqualTo(expected));
        }
    }
}