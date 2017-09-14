using System;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using MaxLossControl.CustomerMovement.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace MasLossControl.CustomerMovement.Tests.Services
{
    [Category("MasLossControl.CustomerMovement.Tests.Services NotificationMovementServiceTests")]
    public class NotificationMovementServiceTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            FanexDataRegisterService.ConfigureFanexData();
        }

        [Test]
        public void Execute_Always_ReturnSuccessfully()
        {
            // Arrange
            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);
            var expected = JobStatus.Success;

            // Act
            var service = container.GetInstance<NotificationMovementService>();
            var result = service.Execute().JobStatus;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}