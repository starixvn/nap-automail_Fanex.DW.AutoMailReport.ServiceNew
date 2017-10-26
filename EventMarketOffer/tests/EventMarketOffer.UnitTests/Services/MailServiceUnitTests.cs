using System;
using System.IO;
using EventMarketOffer.Business.Models;
using EventMarketOffer.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace EventMarketOffer.UnitTests.Services
{
    [Category("EventMarketOffer.UnitTests.Services MailServiceUnitTests")]
    public class MailServiceUnitTests
    {
        private FileService fileService;
        private ExcelService excelService;
        private MailStationService mailStationService;
        private DateService dateService;

        private AppConfigurationService appConfig;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            fileService = Substitute.For<FileService>();
            excelService = Substitute.For<ExcelService>();
            mailStationService = Substitute.For<MailStationService>();
            appConfig = Substitute.For<AppConfigurationService>();
            dateService = Substitute.For<DateService>();
        }

        [Test]
        public void Execute_Always_CreateMailSuccessfully()
        {
            // Arrange
            var fileName = @"Templates\Template.xlsx";
            excelService.Export().Returns(excelService);
            excelService.Save().Returns(fileName);
            appConfig.Configurations.Returns(new AppConfig
            {
                Subject = "Automail report - Event Market Offered",
                Title = "Auto Generated Report Administrator"
            });

            dateService.DateStart.Returns(new DateTime(2017, 07, 01));
            dateService.DateEnd.Returns(new DateTime(2017, 07, 30));
            fileService.ReadContentWhenDataIsAvailable(Arg.Any<string>()).Returns("Content");

            // Act
            var service = new MailServiceImp(fileService, excelService, appConfig, mailStationService, dateService);
            var result = service.Execute();

            // Assert
            Assert.That(result.Subject, Is.EqualTo("Automail report - Event Market Offered from 01 Jul 2017 to 30 Jul 2017"));
            Assert.That(result.Title, Is.EqualTo("Auto Generated Report Administrator"));
        }

        [Test]
        public void SendMailWhenDataIsNotReady_FunctionIdNodataLargeZero_MethodSendShouldBeCalled()
        {
            // Arrange
            appConfig.Configurations.Returns(new AppConfig
            {
                FunctionIdNodata = 90
            });

            // Act
            var service = new MailServiceImp(fileService, excelService, appConfig, mailStationService, dateService);
            service.SendMailWhenDataIsNotReady();

            // Assert
            mailStationService.Received().Send(Arg.Any<Mail>());
        }
    }
}