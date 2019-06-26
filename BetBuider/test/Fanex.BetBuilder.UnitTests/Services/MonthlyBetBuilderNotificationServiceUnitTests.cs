using System.Collections.Generic;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Fanex.MailDelivery.Core.Models;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.MonthlyBetBuilderNotificationServiceUnitTests")]
    public class MonthlyBetBuilderNotificationServiceUnitTests
    {
        private Log log;
        private DataService dataService;
        private TransformService transformService;
        private ExcelService excelService;
        private MailService mailService;

        [SetUp]
        public void Init()
        {
            log = Substitute.For<Log>();
            dataService = Substitute.For<DataService>();
            transformService = Substitute.For<TransformService>();
            excelService = Substitute.For<ExcelService>();
            mailService = Substitute.For<MailService>();
        }

        [Test]
        public void Execute_Always_ReturnMailNoAttachement()
        {
            // Arrange
            dataService.QueryMonthlyData().Returns(new List<MonthlyRawDataModel>());
            transformService.ExecuteMonthly(Arg.Compat.Any<List<MonthlyRawDataModel>>()).Returns(new List<MonthlyDataModel>());
            mailService.SendMonthlyMail(Arg.Compat.Any<string>(), Arg.Compat.Any<bool>()).Returns(new Mail() { Body = "body" });
            var expected = JobStatus.Success;

            // Act
            var service = new MonthlyBetBuilderNotificationServiceImp(log, dataService, transformService, mailService, excelService);
            var mail = service.Execute();
            var result = mail.JobStatus;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}