using System;
using System.Linq;
using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Data.Repository;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsDaily.DatabaseTests.Services
{
    [Category("BetradarVirtualSportsDaily.DatabaseTests.Services DataServiceDatabaseTests")]
    public class DataServiceDatabaseTests
    {
        [Test]
        public void QueryDaily_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            FanexDataRegisterService.ConfigureFanexData();
            var appConfigService = Substitute.For<ApplicationConfigurationService>();            

            // Act
            var service = new RepositoryServiceImp(new DynamicRepository(), appConfigService);
            var result = service.QueryDaily(new DateTime(2017, 07, 01));

            // Assert
            Assert.That(result.Item1.Any());
            Assert.That(result.Item2.Any());
        }
    }
}