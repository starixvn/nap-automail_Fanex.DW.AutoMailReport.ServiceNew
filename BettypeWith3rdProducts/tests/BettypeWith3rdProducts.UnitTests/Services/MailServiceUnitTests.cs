using System;
using System.IO;
using BettypeWith3rdProducts.Business.Services;
using Fanex.MailDelivery.Core.Models;
using NSubstitute;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services MailServiceUnitTests")]
    public class MailServiceUnitTests
    {
        private readonly string fileName = @"Templates\Bettype_With_3rd_Products_Monthly_Winloss_For_2017_06_03.xlsx";

        private ExcelService excelService;
        private MailContentBuilder mailContentBuilder;
        private MailStationService mailStationService;
        private ApplicationConfigurationService appConfig;        

        private void InitializeSubstitute()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            excelService = Substitute.For<ExcelService>();
            mailContentBuilder = Substitute.For<MailContentBuilder>();
            mailStationService = Substitute.For<MailStationService>();
            appConfig = Substitute.For<ApplicationConfigurationService>();
        }

        [Test]
        public void Execute_FunctionIdWithoutAttachmentLargerZero_MethodSendShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            appConfig.FunctionIdWithoutAttachment.Returns(10);
            excelService.Save().Returns(fileName);

            // Act
            var service = new MailServiceImp(excelService, mailContentBuilder, mailStationService, appConfig);
            service.Execute();

            // Assert
            mailStationService.Received().Send(Arg.Any<Mail>());
        }

        [Test]
        public void Execute_Always_MethodReadContentWithAttachmentShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            excelService.Save().Returns(fileName);

            // Act
            var service = new MailServiceImp(excelService, mailContentBuilder, mailStationService, appConfig);
            var result = service.Execute();

            // Assert
            mailContentBuilder.Received().ReadContentWithAttachment(Arg.Any<string>());
        }

        [Test]
        public void SendMailWhenDataIsNotReady_Always_MethodSendShouldBeCalled()
        {
            // Arrange
            InitializeSubstitute();
            appConfig.FunctionIdNodata.Returns(10);

            // Act
            var service = new MailServiceImp(excelService, mailContentBuilder, mailStationService, appConfig);
            service.SendMailWhenDataIsNotReady();

            // Assert
            mailStationService.Received().Send(Arg.Any<Mail>());
        }
    }
}