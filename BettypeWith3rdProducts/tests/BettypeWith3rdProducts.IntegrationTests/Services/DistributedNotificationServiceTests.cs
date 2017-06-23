using BettypeWith3rdProducts.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NUnit.Framework;
using SimpleInjector;

namespace BettypeWith3rdProducts.IntegrationTests.Services
{
    [Category("BettypeWith3rdProducts.IntegrationTests.Services DistributedNotificationServiceTests")]
    public class DistributedNotificationServiceTests
    {
        private DistributedNotificationService service;

        [OneTimeSetUp]
        public void Initialize()
        {
            FanexDataRegisterService.ConfigureFanexData();
            var container = new Container();
            SimpleInjectorService.Register(container);
            service = container.GetInstance<DistributedNotificationService>();
        }

        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            Initialize();

            // Act
            var result = service.Execute();

            // Assert
            Assert.That(result != null);
        }
    }
}