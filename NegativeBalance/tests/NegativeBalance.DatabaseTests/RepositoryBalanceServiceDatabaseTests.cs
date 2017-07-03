using System.Collections.Generic;
using Fanex.Data.Repository;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace NegativeBalance.DatabaseTests
{
    [Category("NegativeBalance.DatabaseTests RepositoryBalanceServiceDatabaseTests")]
    public class RepositoryBalanceServiceDatabaseTests
    {
        private IDynamicRepository dynamicRepository;
        private TransformService transformService;

        [OneTimeSetUp]
        public void Initialize()
        {
            var container = new Container();
            FanexDataRegisterService.ConfigureFanexData();
            NegativeBalanceInjector.Register(container);
            dynamicRepository = container.GetInstance<IDynamicRepository>();
            transformService = container.GetInstance<TransformService>();            
        }

        [Test]
        public void QuerySites_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            var expected = true;

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var mailList = service.QuerySites();
            var result = mailList.Count > 0;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void QueryCustomerBalanceInfoBySite_Always_ConnectToDatabaseSuccessfully()
        {
            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var result = service.QueryCustomerBalanceInfoBySite("12bet", true);

            // Assert
            Assert.That(result.Count >= 0);
        }

        [Test]
        public void QueryMatchSettledLog_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            var matchId = 15314742;
            var groupId = 1;

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var result = service.QueryMatchSettledLog(matchId, groupId);

            // Assert
            Assert.That(result.Item1.GetType() == typeof(MatchInfo));
            Assert.That(result.Item2.GetType() == typeof(List<MatchSettleLog>));
        }

        [Test]
        public void UpdateMatchSent_Always_UpdateSuccessfully()
        {
            // Arrange
            var logsId = "1234";
            var expected = true;

            // Act
            var service = new RepositoryBalanceServiceImp(dynamicRepository, transformService);
            var result = service.UpdateMatchSent(logsId);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}