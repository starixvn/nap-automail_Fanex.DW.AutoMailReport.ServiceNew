using System;
using System.Linq;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace BetradarVirtualSportsMonthly.DatabaseTests.Services
{
    [Category("BetradarVirtualSportsMonthly.DatabaseTests.Services DataServiceDatabaseTests")]
    public class DataServiceDatabaseTests
    {
        public void QueryDaily_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            FanexDataRegisterService.ConfigureFanexData();
            var appConfigService = Substitute.For<ApplicationConfigurationService>();
            var fromDate = new DateTime(2017, 07, 01);
            var toDate = new DateTime(2017, 07, 31);

            // Act
            var service = new RepositoryServiceImp(new DynamicRepository(), appConfigService);
            var result = service.QueryMonthly(fromDate, toDate);
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            // Assert
            Assert.That(result.Item1.Any());
            Assert.That(result.Item2.Any());
        }
    }
}