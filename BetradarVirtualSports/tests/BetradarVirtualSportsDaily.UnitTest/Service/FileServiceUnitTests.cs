using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsDaily.UnitTest.Service
{
    [Category("BetradarVirtualSportsDaily.UnitTest.Service FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void ReadContentWhenDataIsAvailable_Always_Successfully()
        {
            // Arrange
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            SystemTime.Set(new DateTime(2017, 01, 02));
            var dateService = Substitute.For<DateService>();
            dateService.Date.Returns(new DateTime(2017, 01, 01));
            var repositoryService = Substitute.For<RepositoryService>();
            repositoryService.QueryLastDate().Returns(new DateTime(2017, 01, 01));

            var expected = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html");

            // Act
            var fileService = new FileServiceImp(dateService, repositoryService);
            var result = fileService.ReadContentWhenDataIsAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}