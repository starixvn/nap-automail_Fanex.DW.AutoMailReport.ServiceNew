using System;
using System.IO;
using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Repositories;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        private IDynamicRepository dynamicRepository;
        private ApplicationConfigurationService appConfigService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            dynamicRepository = Substitute.For<IDynamicRepository>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
        }

        [Test]
        public void IsDataAvailable_SetManualIsTrue_AlwaysReturnTrue()
        {
            // Arrange
            var expected = true;
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = true;
            appConfigService.Configurations.IsCheckEnoughData = true;

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_SetCheckEnoughDataIsFalse_AlwaysReturnTrue()
        {
            // Arrange
            var expected = true;
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            appConfigService.Configurations.IsCheckEnoughData = false;

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_QueryLastDate_AlwaysReturnTrue()
        {
            // Arrange
            var expected = true;
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            appConfigService.Configurations.IsCheckEnoughData = true;
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 01, 01) });
            SystemTime.Set(new DateTime(2017, 01, 02));

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void QueryLastDate_Always_MethodGetShouldBeCalled()
        {
            // Arrange
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel());

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            service.QueryLastDate();

            // Assert
            dynamicRepository.Received().Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>());
        }

        [Test]
        public void QueryMonthly_Always_MethodShouldBeCalled()
        {
            // Arrange
            var fromdate = new DateTime(2019, 05, 01);
            var todate = new DateTime(2019, 05, 01);

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            service.QueryMonthly(fromdate, todate);

            // Assert
            dynamicRepository.Received().FetchMultiple<GrossWinningMonthly, GrossWinningMonthly>(Arg.Any<MonthlyCriteria>());
        }
    }
}