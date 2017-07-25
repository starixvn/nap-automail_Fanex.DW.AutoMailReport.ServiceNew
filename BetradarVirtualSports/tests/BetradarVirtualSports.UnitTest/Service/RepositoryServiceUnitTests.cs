using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Repositories;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service RepositoryServiceUnitTests")]
    public class RepositoryServiceUnitTests
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
    }
}