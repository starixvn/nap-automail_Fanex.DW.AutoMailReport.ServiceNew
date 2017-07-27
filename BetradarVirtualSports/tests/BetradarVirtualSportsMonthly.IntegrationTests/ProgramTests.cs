using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NUnit.Framework;
using SimpleInjector;

namespace BetradarVirtualSportsMonthly.IntegrationTests
{
    [Category("BetradarVirtualSportsMonthly.IntegrationTests ProgramTests")]
    public class ProgramTests
    {
        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            FanexDataRegisterService.ConfigureFanexData();
            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);
            var expected = JobStatus.Success;

            // Act
            var service = container.GetInstance<DistributedNotificationService>();
            var result = service.Execute();

            // Assert
            Assert.That(result.JobStatus, Is.EqualTo(expected));
        }
    }
}