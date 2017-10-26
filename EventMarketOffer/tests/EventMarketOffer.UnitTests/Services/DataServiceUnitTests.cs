using System;
using System.Data;
using EventMarketOffer.Business.Models;
using EventMarketOffer.Business.Repositories;
using EventMarketOffer.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace EventMarketOffer.UnitTests.Services
{
    [Category("EventMarketOffer.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        private DateService dateService;
        private DataSourceService dataSourceService;
        private IDynamicRepository dynamicRepository;
        private AppConfigurationService appConfig;

        [SetUp]
        public void Initialize()
        {
            dateService = Substitute.For<DateService>();
            dataSourceService = Substitute.For<DataSourceService>();
            dynamicRepository = Substitute.For<DynamicRepository>();
            appConfig = Substitute.For<AppConfigurationService>();
        }

        [Test]
        public void Query_Always_MethodQueryShouldbeCalled()
        {
            // Arrange
            dataSourceService.Query(Arg.Any<CriteriaEventMartketOffer>()).Returns(new DataTable());

            // Act
            var service = new DataServiceImp(dateService, dataSourceService, dynamicRepository, appConfig);
            var result = service.Query();

            // Assert
            dataSourceService.Received().Query(Arg.Any<CriteriaEventMartketOffer>());
        }

        [Test]
        public void QueryLastDate_Always_MethodShouldBeCalled()
        {
            // Arrange
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel());

            // Act
            var service = new DataServiceImp(dateService, dataSourceService, dynamicRepository, appConfig);
            service.QueryLastDate();

            // Assert
            dynamicRepository.Received().Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>());
        }

        [Test]
        public void IsDataAvailable_SetIsManualTrue_AlwaysReturnTrue()
        {
            // Arrange
            appConfig.Configurations.Returns(new AppConfig { IsManual = true });
            var expected = true;

            // Act
            var service = new DataServiceImp(dateService, dataSourceService, dynamicRepository, appConfig);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_SetIsCheckEnoughDataFalse_AlwaysReturnTrue()
        {
            // Arrange
            appConfig.Configurations.Returns(new AppConfig { IsManual = false, IsCheckEnoughData = false });
            var expected = true;

            // Act
            var service = new DataServiceImp(dateService, dataSourceService, dynamicRepository, appConfig);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_CheckCondition_AlwaysReturnTrue()
        {
            // Arrange
            appConfig.Configurations.Returns(new AppConfig { IsManual = false, IsCheckEnoughData = true });
            SystemTime.Set(new DateTime(2017, 09, 10));
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 09, 10) });
            var expected = true;

            // Act
            var service = new DataServiceImp(dateService, dataSourceService, dynamicRepository, appConfig);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_CheckCondition_AlwaysReturnFalse()
        {
            // Arrange
            appConfig.Configurations.Returns(new AppConfig { IsManual = false, IsCheckEnoughData = true });
            SystemTime.Set(new DateTime(2017, 09, 10));
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 09, 08) });
            var expected = false;

            // Act
            var service = new DataServiceImp(dateService, dataSourceService, dynamicRepository, appConfig);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}