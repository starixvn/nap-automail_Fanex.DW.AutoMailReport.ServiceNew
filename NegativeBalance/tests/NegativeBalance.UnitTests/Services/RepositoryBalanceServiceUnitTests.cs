using System;
using System.Collections.Generic;
using Fanex.Data.Repository;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Repositories;
using NegativeBalance.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services RepositoryBalanceServiceUnitTests")]
    public class RepositoryBalanceServiceUnitTests
    {
        private IDynamicRepository dynamicRepository;
        private TransformService transformService;

        private void Initialize()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            transformService = Substitute.For<TransformService>();
        }

        [Test]
        public void QuerySites_Always_ReturnSitesAvailable()
        {
            // Arrange
            Initialize();
            var sites = new List<Site>
            {
                new Site { LicenseeName = "12bet", IsAvailable = true },
                new Site { LicenseeName = "m88", IsAvailable = false }
            };

            dynamicRepository.Fetch<Site>(Arg.Any<SelectMailingListCriteria>()).Returns(sites);
            var expected = new List<Site> { new Site { LicenseeName = "12bet", IsAvailable = true } };

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var result = service.QuerySites(false);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void QueryCustomerBalanceInfoBySite_Always_ReturnRightData()
        {
            // Arrange
            Initialize();
            var customers = new List<CustomerBalanceInfo>
            {
                new CustomerBalanceInfo { UserName = "user1" },
                new CustomerBalanceInfo { UserName = "user2" },
            };

            dynamicRepository.Fetch<CustomerBalanceInfo>(Arg.Any<SelectBySiteCriteria>()).Returns(customers);

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            service.QueryCustomerBalanceInfoBySite("12bet", true);

            // Assert
            dynamicRepository.Received().Fetch<CustomerBalanceInfo>(Arg.Any<SelectBySiteCriteria>());
        }

        [Test]
        public void QueryMatchSettledLog_Always_ReturnRightData()
        {
            // Arrange
            Initialize();
            var rawMatchInfo = new List<RawMatchInfo>();
            var matchSettleLog = new List<MatchSettleLog>();
            dynamicRepository.FetchMultiple<RawMatchInfo, MatchSettleLog>(Arg.Any<SelectMatchLogCriteria>()).Returns(new Tuple<IEnumerable<RawMatchInfo>, IEnumerable<MatchSettleLog>>(rawMatchInfo, matchSettleLog));
            transformService.Execute(Arg.Any<string>(), Arg.Any<RawMatchInfo>(), Arg.Any<int>()).Returns(new MatchInfo());

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var result = service.QueryMatchSettledLog("12bet", 1, 2);

            // Assert
            Assert.That(result.Item1.GetType(), Is.EqualTo(typeof(MatchInfo)));
            Assert.That(result.Item2.GetType(), Is.EqualTo(typeof(List<MatchSettleLog>)));
        }

        [Test]
        public void UpdateMatchSent_Always_UpdateStatusLogSuccessfully()
        {
            // Arrange
            Initialize();

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var result = service.UpdateMatchSent("1,2");

            // Assert
            dynamicRepository.Received().Execute(Arg.Any<UpdateMatchSentCommand>());
        }
    }
}