using System;
using System.Collections.Generic;
using System.IO;
using CustomerMovement.Business.Models;
using CustomerMovement.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace CustomerMovement.UnitTests.Services
{
    [Category("CustomerMovement.UnitTests.Services NotificationMovementServiceUnitTests")]
    public class NotificationMovementServiceUnitTests
    {
        private MailMovementService mailMovementService;
        private LogService logService;
        private DataService dataService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            mailMovementService = Substitute.For<MailMovementService>();
            logService = Substitute.For<LogService>();
            dataService = Substitute.For<DataService>();
        }

        [Test]
        public void Execute_ThrowException_MethodLogShouldBeCalled()
        {
            // Arrange
            dataService.Query().Returns(x => { throw new Exception(); });

            // Act
            var service = new NotificationMovementServiceImp(mailMovementService, logService, dataService);
            service.Execute();

            // Assert
            logService.Received().Log(Arg.Any<Exception>());
        }

        [Test]
        public void Execute_Always_Successully()
        {
            // Arrange
            var expected = JobStatus.Success;

            // Act
            var service = new NotificationMovementServiceImp(mailMovementService, logService, dataService);
            var jobMessage = service.Execute();
            var result = jobMessage.JobStatus;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Execute_Always_MethodsShouldBeCalled()
        {
            // Arrange

            // Act
            var service = new NotificationMovementServiceImp(mailMovementService, logService, dataService);
            service.Execute();

            // Assert
            dataService.Received().Query();
            mailMovementService.Received().Execute(Arg.Any<Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>>>());
        }
    }
}