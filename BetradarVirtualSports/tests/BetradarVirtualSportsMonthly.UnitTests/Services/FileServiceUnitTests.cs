using System;
using System.IO;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services FileServiceUnitTests")]
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
        public void ReadContentWhenDataIsAvailable_Always_ReturnRightContent()
        {
            // Arrange
            Initialize();
            dateService.FromDate.Returns(new DateTime(2017, 05, 01));
            dateService.ToDate.Returns(new DateTime(2017, 05, 31));
            var expected = TestLoader.DataLoader("ContentWithAttachment-expected.txt");

            // Act
            var service = new FileServiceImp(dateService, repositoryService);
            var result = service.ReadContentWhenDataIsAvailable();

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