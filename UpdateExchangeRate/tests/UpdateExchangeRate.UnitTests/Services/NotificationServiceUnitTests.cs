using System;
using System.Collections.Generic;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using Fanex.UpdateExchangeRate.Business.Models;
using Fanex.UpdateExchangeRate.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace UpdateExchangeRate.UnitTests.Services
{
    [Category("UpdateExchangeRate.UnitTests.Services NotificationServiceUnitTests")]
    public class NotificationServiceUnitTests
    {
        private Fanex.UpdateExchangeRate.Business.Services.MailService mailService;
        private LogService logService;
        private DataService dataService;

        [SetUp]
        public void Initial()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            mailService = Substitute.For<Fanex.UpdateExchangeRate.Business.Services.MailService>();
            logService = Substitute.For<LogService>();
            dataService = Substitute.For<DataService>();
        }

        [Test]
        public void Execute_Nodata_MailShouldNotBeSent()
        {
            // Arrange
            dataService.Query().Returns(new List<CurrencyRate>());
            var expected = JobStatus.SuccessWithoutSendmail;

            // Act
            var service = new NotificationServiceImp(mailService, logService, dataService);
            var result = service.Execute();

            // Assert
            Assert.That(result.JobStatus, Is.EqualTo(expected));
        }

        [Test]
        public void Execute_Always_MailShouldBeSentSuccessfully()
        {
            // Arrange
            dataService.Query().Returns(new List<CurrencyRate> { new CurrencyRate() });
            mailService.Execute(Arg.Any<List<CurrencyRate>>()).Returns(new Mail());
            var expected = JobStatus.Success;

            // Act
            var service = new NotificationServiceImp(mailService, logService, dataService);
            var result = service.Execute();

            // Assert
            Assert.That(result.JobStatus, Is.EqualTo(expected));
        }
    }
}