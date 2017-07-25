using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service DistributedNotificationUnitTests")]
    public class MailServiceUnitTests
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
            var fileName = @"Templates\Daily-Template.xlsx";
            SystemTime.Set(new DateTime(2017, 01, 02));
            var appConfig = TestLoader.DataLoader<AppConfig>("AppConfig.json");
            appConfig.IsManual = false;
            appConfigService.Configurations.Returns(appConfig);
            var body = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html");
            fileService.ReadContentWhenDataIsAvailable().Returns(body);
            excelService.Export().Save().Returns(fileName);
            dateService.Date.Returns(new DateTime(2017, 01, 01));
            var expected = new Mail
            {
                Subject = $"[Betradar Virtual Sports] - Daily Report {SystemTime.Now.AddDays(-1):MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = body,
                Attachments = new List<MailAttachment> { new MailAttachment(fileName, FileExtension.MS_EXCEL) }
            };

            // Act
            var service = new MailDailyService(appConfigService, fileService, excelService, dateService, mailStationService);
            var result = service.Execute();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [TearDown]
        public void Reset()
        {
            SystemTime.Reset();
        }
    }
}