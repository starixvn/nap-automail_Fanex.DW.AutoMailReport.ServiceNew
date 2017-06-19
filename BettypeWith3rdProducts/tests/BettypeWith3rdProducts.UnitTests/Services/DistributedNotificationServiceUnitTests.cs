using System;
using BettypeWith3rdProducts.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DistributedNotificationServiceUnitTests")]
    public class DistributedNotificationServiceUnitTests
    {
        private MailService mailService;
        private LogService logService;
        private DataService dataService;

        private void InitializeSubstitute()
        {
            mailService = Substitute.For<MailService>();
            logService = Substitute.For<LogService>();
            dataService = Substitute.For<DataService>();
        }

        [Test]
        public void Execute_DataNotAvailable_MethodSendMailWhenDataIsNotReadyShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            dataService.IsDataAvailable().Returns(false);

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, dataService);
            service.Execute();

            // Assert
            mailService.Received().SendMailWhenDataIsNotReady();
        }

        [Test]
        public void Execute_DataNotAvailable_AlwaysReturnJobStatusDataIsNotReady()
        {
            // Arrange
            InitializeSubstitute();
            dataService.IsDataAvailable().Returns(false);
            var expected = JobStatus.DataIsNotReady;

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, dataService);
            var result = service.Execute().JobStatus;

            // Assert
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void Execute_DataAvailable_MethodMailServiceExecuteShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            dataService.IsDataAvailable().Returns(true);

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, dataService);
            service.Execute();

            // Assert
            mailService.Received().Execute();
        }

        [Test]
        public void Execute_DataAvailable_AlwaysReturnJobStatusSuccess()
        {
            // Arrange
            InitializeSubstitute();
            dataService.IsDataAvailable().Returns(true);
            var expected = JobStatus.Success;

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, dataService);
            var result = service.Execute().JobStatus;

            // Assert
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void Execute_ThrowException_ExceptionAlwaysThrow()
        {
            // Arrange
            InitializeSubstitute();
            dataService.IsDataAvailable().Returns(x => { throw new Exception(); });

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, dataService);
            service.Execute();

            // Assert
            Assert.Throws<Exception>(() => dataService.IsDataAvailable());
        }

        [Test]
        public void Execute_ThrowException_MethodLogShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            dataService.IsDataAvailable().Returns(x => { throw new Exception(); });

            // Act
            var service = new DistributedNotificationServiceImp(mailService, logService, dataService);
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