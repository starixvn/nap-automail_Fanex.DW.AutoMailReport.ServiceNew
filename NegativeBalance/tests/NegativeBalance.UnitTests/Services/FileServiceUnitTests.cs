using System;
using System.Collections.Generic;
using System.IO;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void ReadContent_Group1_ReturnRightContent()
        {
            // Arrange
            var matchLogGroup1 = TestLoader.DataLoader<List<Match>>("MatchSettledLogGroup1-input.json");
            var expected = TestLoader.DataLoader("MatchSettledLogGroup1-expected.txt");

            // Act
            var service = new FileServiceImp();
            var result = service.ReadContent(matchLogGroup1);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ReadContent_Group3_ReturnRightContent()
        {
            // Arrange
            var matchLogGroup3 = TestLoader.DataLoader<List<Match>>("MatchSettledLogGroup3-input.json");
            var expected = TestLoader.DataLoader("MatchSettledLogGroup3-expected.txt");

            // Act
            var service = new FileServiceImp();
            var result = service.ReadContent(matchLogGroup3);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ReadContent_Group7_ReturnRightContent()
        {
            // Arrange
            var matchLogGroup7 = TestLoader.DataLoader<List<Match>>("MatchSettledLogGroup7-input.json");
            var expected = TestLoader.DataLoader("MatchSettledLogGroup7-expected.txt");

            // Act
            var service = new FileServiceImp();
            var result = service.ReadContent(matchLogGroup7);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}