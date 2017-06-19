using System;
using System.Collections.Generic;
using System.IO;
using BettypeWith3rdProducts.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services MailContentBuilderUnitTests")]
    public class MailContentBuilderUnitTests
    {
        private ApplicationConfigurationService appConfig;
        private FileService fileService;

        private void InitializeSubstitute()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            appConfig = Substitute.For<ApplicationConfigurationService>();
            fileService = Substitute.For<FileService>();
            appConfig.Subject.Returns("Automail Report - Bettype With 3rd Products");
            SystemTime.Set(new DateTime(2017, 01, 01));
            appConfig.Title.Returns("Auto Generated Report Administrator");
            appConfig.FunctionIdNodata.Returns(10);
        }

        [Test]
        public void ReadContentWhenDataIsNotReady_Always_ReturnRightContent()
        {
            // Arrange
            InitializeSubstitute();
            var contentNodata = TestLoader.DataLoader("ContentWhenDataIsNotReady-Expected.txt");
            fileService.ReadContentWhenDataIsNotReady().Returns(contentNodata);
            var expected = CreateMailWhenDataIsNotReady();

            // Act
            var service = new MailContentBuilderImp(appConfig, fileService);
            var result = service.ReadContentWhenDataIsNotReady();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private Mail CreateMailWhenDataIsNotReady()
        {
            return new Mail
            {
                Subject = "Automail Report - Bettype With 3rd Products - 2017/01/01",
                Title = "Auto Generated Report Administrator",
                Body = TestLoader.DataLoader("ContentWhenDataIsNotReady-Expected.txt"),
                Attachments = new List<MailAttachment>(),
                MailingListId = 10
            };
        }

        [Test]
        public void ReadContentWithAttachment_Always_ReturnRightContent()
        {
            InitializeSubstitute();
            var contentNodata = TestLoader.DataLoader("ContentWithAttachment-expected.txt");
            fileService.ReadContentWhenDataIsAvailable(Arg.Any<string>(), Arg.Any<string>()).Returns(contentNodata);
            var fileName = @"Templates\Bettype_With_3rd_Products_Monthly_Winloss_For_2017_06_03.xlsx";
            var expected = CreateMailWithAttachment();

            // Act
            var service = new MailContentBuilderImp(appConfig, fileService);
            var result = service.ReadContentWithAttachment(fileName);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private Mail CreateMailWithAttachment()
        {
            var fileName = @"Templates\Bettype_With_3rd_Products_Monthly_Winloss_For_2017_06_03.xlsx";
            return new Mail
            {
                Subject = "Automail Report - Bettype With 3rd Products - 2017/01/01",
                Title = "Auto Generated Report Administrator",
                Body = TestLoader.DataLoader("ContentWithAttachment-Expected.txt"),
                Attachments = new List<MailAttachment> { new MailAttachment(fileName, FileExtension.MS_EXCEL) },
                MailingListId = 0
            };
        }

        [Test]
        public void ReadContentWithoutAttachment_Always_ReturnRightContent()
        {
            InitializeSubstitute();
            var contentNodata = TestLoader.DataLoader("ContentWithoutAttachment-expected.txt");
            fileService.ReadContentWhenDataIsAvailable(Arg.Any<string>(), Arg.Any<string>()).Returns(contentNodata);
            var fileName = @"Templates\Bettype_With_3rd_Products_Monthly_Winloss_For_2017_06_03.xlsx";
            var expected = CreateMailWithoutAttachment();

            // Act
            var service = new MailContentBuilderImp(appConfig, fileService);
            var result = service.ReadContentWithoutAttachment(fileName);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private Mail CreateMailWithoutAttachment()
        {
            return new Mail
            {
                Subject = "Automail Report - Bettype With 3rd Products - 2017/01/01",
                Title = "Auto Generated Report Administrator",
                Body = TestLoader.DataLoader("ContentWithoutAttachment-Expected.txt"),
                Attachments = new List<MailAttachment>(),
                MailingListId = 0
            };
        }
    }
}