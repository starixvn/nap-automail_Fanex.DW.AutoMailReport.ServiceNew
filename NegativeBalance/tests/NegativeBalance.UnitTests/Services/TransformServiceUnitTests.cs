using System;
using System.Configuration;
using Fanex.RemotingRef;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services TransformServiceUnitTests")]
    public class TransformServiceUnitTests
    {
        private IRemotingRefService remotingRefService;

        [SetUp]
        public void Init()
        {
            RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["RemoteRefURL"]))
                .FromApplication("Fanex.NegativeBalance.Service")
                .RegisterSupportedLanguages("en-US", "zh-CN");

            remotingRefService = new RemotingRefService();
        }

        [Test]
        public void Execute_Always_ReturnRightContent()
        {
            // Arrange
            var rawMatchInfo = CreateRawMatchInfo();
            var expected = TestLoader.DataLoader<MatchInfo>("MatchInfo-expected.json");

            // Act
            var service = new TransformServiceImp(remotingRefService);
            var result = service.Execute("12bet", rawMatchInfo, 1);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private RawMatchInfo CreateRawMatchInfo()
        {
            return new RawMatchInfo
            {
                MatchId = 15314742,
                LeagueId = 165,
                MatchCode = "991030MR2",
                EventDate = new DateTime(2015, 11, 10),
                HomeId = 15700,
                AwayId = 15701,
                SportId = 1
            };
        }

        [Test]
        public void Execute_rawMatchInfoNull_ReturnDefaultContent()
        {
            // Arrange
            var rawMatchInfo = CreateRawMatchInfo();
            var expected = new MatchInfo();

            // Act
            var service = new TransformServiceImp(remotingRefService);
            var result = service.Execute("12bet", null, 1);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void Execute_Always_WriteLogShuoldBeCalled()
        {
            // Arrange
            var rawMatchInfo = new RawMatchInfo { MatchCode = "123" };

            // Act
            var service = new TransformServiceImp(remotingRefService);
            var result = service.Execute("12bet", rawMatchInfo, 1);

            // Assert
            Assert.That(result != null);
        }
    }
}