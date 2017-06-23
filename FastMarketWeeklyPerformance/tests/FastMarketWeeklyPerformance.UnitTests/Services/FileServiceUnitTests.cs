using System;
using System.IO;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        private RepositoryService repositoryService;
        private ApplicationConfigurationService appConfigService;
        private DateRangeService dateRangeService;
        private DateRange dateRange;

        private void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            repositoryService = Substitute.For<RepositoryService>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            dateRangeService = Substitute.For<DateRangeService>();
            dateRange = Substitute.For<DateRange>();
        }

        [Test]
        public void ReadContentWhenDataIsNotReady_Always_ReturnRightContent()
        {
            // Arrange
            Initialize();
            repositoryService.QueryLastDate().Returns(new DateTime(2017, 01, 01));
            SystemTime.Set(new DateTime(2017, 01, 10));
            var expected = TestLoader.DataLoader("ReadContentWhenDataIsNotReady-expected.html");

            // Act
            var service = new FileServiceImp(repositoryService, appConfigService, dateRangeService);
            var result = service.ReadContentWhenDataIsNotReady();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ReadContentWhenDataIsAvailable_Always_ReturnRightContent()
        {
            // Arrange
            Initialize();
            appConfigService.Configurations.Returns(new AppConfig());
            dateRange.ToDate.Returns(new DateTime(2017, 01, 01));
            dateRangeService.DateRange(Arg.Any<ReportDateRange>()).Returns(dateRange);
            var expected = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html");

            // Act
            var service = new FileServiceImp(repositoryService, appConfigService, dateRangeService);
            var result = service.ReadContentWhenDataIsAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}