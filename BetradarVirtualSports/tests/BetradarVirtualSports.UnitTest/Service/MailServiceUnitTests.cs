using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSports.Business.Models;
using BetradarVirtualSports.Business.Services;
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

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            fileService = Substitute.For<FileService>();
        }

        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var appConfig = TestLoader.DataLoader<AppConfig>("AppConfig.json");
            appConfigService.Configurations.Returns(appConfig);
            var body = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html");
            fileService.ReadContentWhenDataIsAvailable().Returns(body);

            var fileName = @"Templates\Betradar_Virtual_Sport_Daily_Report.xlsx";
            var expected = new Mail
            {
                Subject = $"[Betradar Virtual Sports] - Daily Report {SystemTime.Now:MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = body,
                Attachments = new List<MailAttachment> { new MailAttachment(fileName, FileExtension.MS_EXCEL) }
            };

            // Act
            var service = new MailServiceImp(appConfigService, fileService);
            var result = service.Execute();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}