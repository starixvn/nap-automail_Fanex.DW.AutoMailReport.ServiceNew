using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Repositories;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsDaily.UnitTests.Service
{
    [Category("BetradarVirtualSportsDaily.UnitTests.Service DataServiceUnitTests")]
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
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = true;
            appConfigService.Configurations.IsCheckEnoughData = true;

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void IsDataAvailable_SetCheckEnoughDataIsFalse_AlwaysReturnTrue()
        {
            // Arrange
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            appConfigService.Configurations.IsCheckEnoughData = false;

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void IsDataAvailable_QueryLastDate_AlwaysReturnTrue()
        {
            // Arrange
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            appConfigService.Configurations.IsCheckEnoughData = true;
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 01, 01) });
            SystemTime.Set(new DateTime(2017, 01, 02));

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void QueryLastDate_Always_MethodGetShouldBeCalled()
        {
            // Arrange
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel());

            // Act
            var service = new RepositoryServiceImp(dynamicRepository, appConfigService);
            service.QueryStatus();

            // Assert
            dynamicRepository.Received().Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>());
        }
    }
}