using System;
using Fanex.MailDelivery.Core.Models;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Service.Core.UnitTests
{
    [Category("Fanex.Service.Core.UnitTests DistributedNotificationServiceUnitTests")]
    public class DistributedNotificationServiceUnitTests
    {
        private MailService mailService;
        private LogService logService;
        private RepositoryService repositoryService;

        private void InitializeSubstitute()
        {
            mailService = Substitute.For<MailService>();
            logService = Substitute.For<LogService>();
            repositoryService = Substitute.For<RepositoryService>();
        }

        [Test]
        public void Execute_DataNotAvailable_MethodSendMailWhenDataIsNotReadyShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(false);

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, repositoryService);
            service.Execute();

            // Assert
            mailService.Received().SendMailWhenDataIsNotReady();
        }

        [Test]
        public void Execute_DataNotAvailable_AlwaysReturnJobStatusDataIsNotReady()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(false);
            var expected = JobStatus.DataIsNotReady;

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, repositoryService);
            var result = service.Execute().JobStatus;

            // Assert
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void Execute_DataAvailable_MethodMailServiceExecuteShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(true);

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, repositoryService);
            service.Execute();

            // Assert
            mailService.Received().Execute();
        }

        [Test]
        public void Execute_DataAvailable_AlwaysReturnJobStatusSuccess()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(true);
            var expected = JobStatus.Success;

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, repositoryService);
            var result = service.Execute().JobStatus;

            // Assert
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void Execute_ThrowException_ExceptionAlwaysThrow()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(x => { throw new Exception(); });

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, repositoryService);
            service.Execute();

            // Assert
            Assert.Throws<Exception>(() => repositoryService.IsDataAvailable());
        }

        [Test]
        public void Execute_ThrowException_MethodLogShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(x => { throw new Exception(); });

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, repositoryService);
            service.Execute();

            // Assert
            logService.Received().Log(Arg.Any<Exception>());
        }

        [TearDown]
        public void Reset()
        {
            SystemTime.Reset();
        }
    }
}