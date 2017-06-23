using System;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Repositories;
using FastMarketWeeklyPerformance.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services RepositoryServiceUnitTests")]
    public class RepositoryServiceUnitTests
    {
        private IDynamicRepository dynamicRepository;
        private ApplicationConfigurationService appConfigService;

        private void Initialize()
        {
            dynamicRepository = Substitute.For<DynamicRepository>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
        }

        [Test]
        public void QueryLastDate_Always_ReturnRightDate()
        {
            // Arrange
            Initialize();
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 01, 01) });
            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.QueryLastDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_ReportDateRangeIsManual_AlwaysReturnTrue()
        {
            // Arrange
            Initialize();
            appConfigService.Configurations.Returns(new AppConfig { ReportDateRange = ReportDateRange.Manual });
            var expected = true;

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_LastDateLargerThanYesterday_AlwaysReturnTrue()
        {
            // Arrange
            Initialize();
            appConfigService.Configurations.Returns(new AppConfig { IsCheckEnoughData = true });
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 01, 01) });
            SystemTime.Set(new DateTime(2017, 01, 02));
            var expected = true;

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}