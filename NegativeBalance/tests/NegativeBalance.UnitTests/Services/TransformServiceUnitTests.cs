using System;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services TransformServiceUnitTests")]
    public class TransformServiceUnitTests
    {
        [Test]
        public void Execute_Always_ReturnRightContent()
        {
            // Arrange
            var rawMatchInfo = CreateRawMatchInfo();
            var expected = TestLoader.DataLoader<MatchInfo>("MatchInfo-expected.json");

            // Act
            var service = new TransformServiceImp();
            var result = service.Execute("12bet", rawMatchInfo);

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
            var service = new TransformServiceImp();
            var result = service.Execute("12bet", null);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}