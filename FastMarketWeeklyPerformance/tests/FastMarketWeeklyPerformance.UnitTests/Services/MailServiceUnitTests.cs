using System;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services MailServiceUnitTests")]
    public class MailServiceUnitTests
    {
        private FileService fileService;
        private ExcelService excelService;
        private MailStationService mailStationService;
        private ApplicationConfigurationService appConfigService;

        private void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            fileService = Substitute.For<FileService>();
            excelService = Substitute.For<ExcelService>();
            mailStationService = Substitute.For<MailStationService>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();

            var appConfig = TestLoader.DataLoader<AppConfig>("AppConfig.json");
            appConfigService.Configurations.Returns(appConfig);
        }

        [Test]
        public void Execute_Always_ReturnRightContent()
        {
            // Arrange
            Initialize();
            excelService.Export().Save().Returns(@"Templates\Template.xlsx");
            fileService.ReadContentWhenDataIsAvailable().Returns(TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html"));

            // Act
            var service = new MailServiceImp(fileService, excelService, appConfigService, mailStationService);
            var result = service.Execute();

            // Assert
            Assert.That(result.Attachments.Count == 1);
        }

        [Test]
        public void SendMailWhenDataIsNotReady_FunctionIdNodataIsZero_MethodSendShouldNotBeCalled()
        {
            // Arrange
            Initialize();
            appConfigService.Configurations.FunctionIdNodata = 0;

            // Act
            var service = new MailServiceImp(fileService, excelService, appConfigService, mailStationService);
            service.SendMailWhenDataIsNotReady();

            // Assert
            mailStationService.DidNotReceive().Send(Arg.Any<Mail>());
        }

        [Test]
        public void SendMailWhenDataIsNotReady_FunctionIdNodataLargerThanZero_MethodSendShouldBeCalled()
        {
            // Arrange
            Initialize();

            // Act
            var service = new MailServiceImp(fileService, excelService, appConfigService, mailStationService);
            service.SendMailWhenDataIsNotReady();

            // Assert
            mailStationService.Received().Send(Arg.Any<Mail>());
        }
    }
}