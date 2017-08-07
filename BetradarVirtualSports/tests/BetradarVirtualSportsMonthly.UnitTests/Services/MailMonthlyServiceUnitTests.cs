using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services MailMonthlyServiceUnitTests")]
    public class MailMonthlyServiceUnitTests
    {
        private ApplicationConfigurationService appConfigService;
        private FileService fileService;
        private ExcelService excelService;
        private DateService dateService;
        private MailStationService mailStationService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            fileService = Substitute.For<FileService>();
            excelService = Substitute.For<ExcelService>();
            dateService = Substitute.For<DateService>();
            mailStationService = Substitute.For<MailStationService>();
        }

        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            var fileName = @"Templates\Monthly-Template.xlsx";
            SystemTime.Set(new DateTime(2017, 01, 02));
            var appConfig = TestLoader.DataLoader<AppConfig>("AppConfig.json");
            appConfig.IsManual = false;
            appConfigService.Configurations.Returns(appConfig);
            var body = TestLoader.DataLoader("ContentWithAttachment-expected.txt");
            fileService.ReadContentWhenDataIsAvailable().Returns(body);
            excelService.Export().Save().Returns(fileName);
            dateService.FromDate.Returns(new DateTime(2017, 01, 01));
            dateService.ToDate.Returns(new DateTime(2017, 01, 31));

            var expected = new Mail
            {
                Subject = $"{appConfig.Subject} {dateService.FromDate:MM/dd/yyyy} - {dateService.ToDate:MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = body,
                Attachments = new List<MailAttachment> { new MailAttachment(fileName, FileExtension.MS_EXCEL) }
            };

            // Act
            var service = new MailMonthlyService(appConfigService, fileService, excelService, dateService, mailStationService);
            var result = service.Execute();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void SendMailWhenDataIsNotReady_Always_Successfully()
        {
            // Arrange
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));

            // Act
            var service = new MailMonthlyService(appConfigService, fileService, excelService, dateService, mailStationService);
            service.SendMailWhenDataIsNotReady();

            // Assert
            mailStationService.Received().Send(Arg.Any<Mail>());
        }

        [TearDown]
        public void Reset()
        {
            SystemTime.Reset();
        }
    }
}