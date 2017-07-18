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

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            appConfigService = Substitute.For<ApplicationConfigurationService>();
        }

        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var appConfig = TestLoader.DataLoader<AppConfig>("AppConfig.json");
            appConfigService.Configurations.Returns(appConfig);
            var fileName = @"Templates\Betradar_Virtual_Sport_Daily_Report.xlsx";
            var expected = new Mail
            {
                Subject = $"[Betradar Virtual Sports] - Daily Report {SystemTime.Now:MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html"),
                Attachments = new List<MailAttachment> { new MailAttachment(fileName, FileExtension.MS_EXCEL) }
            };

            // Act
            var service = new MailServiceImp(appConfigService);
            var result = service.Execute();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}