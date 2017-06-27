using System;
using System.Collections.Generic;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        private RepositoryBalanceService repository;
        private ApplicationConfigurationService appConfigService;

        private void Initialize()
        {
            repository = Substitute.For<RepositoryBalanceService>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
        }

        [Test]
        public void QueryMatchesBySite_Always_ReturnRightData()
        {
            // Arrange
            Initialize();
            var customers = TestLoader.DataLoader<List<CustomerBalanceInfo>>("Repository-QueryCustomerBalanceInfoBySite.json");
            appConfigService.Configurations.Returns(new AppConfig());
            repository.QueryCustomerBalanceInfoBySite(Arg.Any<string>(), Arg.Any<bool>()).Returns(customers);

            var matchSettledLog = TestLoader.DataLoader<Tuple<MatchInfo, List<MatchSettleLog>>>("Repository-QueryMatchSettledLog.json");
            repository.QueryMatchSettledLog(Arg.Any<int>(), Arg.Any<int>()).Returns(matchSettledLog);
            var expected = TestLoader.DataLoader<Tuple<List<Match>, string>>("DataService-QueryMatchesBySite-expected.json");

            // Act
            var service = new DataServiceImp(repository, appConfigService);
            var result = service.QueryMatchesBySite(new Site { LicenseeName = "12bet" });

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}