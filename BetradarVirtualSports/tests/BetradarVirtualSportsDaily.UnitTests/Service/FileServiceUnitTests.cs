using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsDaily.UnitTests.Service
{
    [Category("BetradarVirtualSportsDaily.UnitTests.Service FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        private DateService dateService;
        private RepositoryService repositoryService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            dateService = Substitute.For<DateService>();
            repositoryService = Substitute.For<RepositoryService>();
        }

        [Test]
        public void ReadContentWhenDataIsAvailable_Always_Successfully()
        {
            // Arrange
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            SystemTime.Set(new DateTime(2017, 01, 02));            
            dateService.Date.Returns(new DateTime(2017, 01, 01));            
            repositoryService.QueryLastDate().Returns(new DateTime(2017, 01, 01));

            var expected = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html");

            // Act
            var fileService = new FileServiceImp(dateService, repositoryService);
            var result = fileService.ReadContentWhenDataIsAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ReadContentWhenDataIsNotReady_Always_ReturnRightContent()
        {
            // Arrange
            Initialize();
            SystemTime.Set(new DateTime(2017, 01, 03));
            repositoryService.QueryLastDate().Returns(new DateTime(2017, 01, 01));
            var expected = TestLoader.DataLoader("ContentWhenDataIsNotReady-expected.txt");

            // Act
            var service = new FileServiceImp(dateService, repositoryService);
            var result = service.ReadContentWhenDataIsNotReady();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}