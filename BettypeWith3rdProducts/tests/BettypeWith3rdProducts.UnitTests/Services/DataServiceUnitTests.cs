using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using BettypeWith3rdProducts.BusinessSubstitute;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        private ApplicationConfigurationService appConfig;
        private SportSplitterService sportSplitterService;
        private RepositoryService repositoryService;

        private void InitializeSubstitute()
        {
            appConfig = Substitute.For<ApplicationConfigurationService>();
            sportSplitterService = Substitute.For<SportSplitterService>();
            repositoryService = Substitute.For<RepositoryService>();
        }

        [Test]
        public void Execute_Always_ReturnRightData()
        {
            // Arrange
            InitializeSubstitute();
            var sourceMappings = TestLoader.DataLoader<List<SourceMapping>>("SourceMapping.json");
            appConfig.SourceMappings.Returns(sourceMappings);
            var dataSource = CreateDataSource();
            sportSplitterService.Execute(Arg.Any<SourceMapping>()).Returns(dataSource);
            var expected = CreateExpected();

            // Act
            var service = new DataServiceImp(appConfig, sportSplitterService, repositoryService);
            var result = service.Execute();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private Dictionary<string, List<DataTable>> CreateDataSource()
        {
            return new Dictionary<string, List<DataTable>>
            {
                { "All", new List<DataTable> { new DataTable() } },
                { "Summary", new List<DataTable> { new DataTable() } },
                { "Soccer", new List<DataTable> { new DataTable() } },
            };
        }

        private Dictionary<string, Dictionary<string, List<DataTable>>> CreateExpected()
        {
            var dataSource = CreateDataSource();
            return new Dictionary<string, Dictionary<string, List<DataTable>>>
            {
                { "Real",  dataSource },
                { "Virtual",  dataSource },
                { "Real Juice & Leftover",  dataSource },
                { "Virtual Juice & Leftover",  dataSource }
            };
        }

        [Test]
        public void IsDataAvailable_DateRangeIsManual_AlwaysReturnTrue()
        {
            // Arrange
            InitializeSubstitute();
            repositoryService.IsDataAvailable().Returns(true);
            var expected = true;

            // Act
            var service = new DataServiceImp(appConfig, sportSplitterService, repositoryService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result == expected);
        }

        [Test]
        public void IsDataAvailable_Substitute_AlwaysReturnTrue()
        {
            // Arrange
            var expected = true;

            // Act
            var service = new DataServiceSubstitute();
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Execute_Substitute_AlwaysReturnRightData()
        {
            // Arrange
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            // Act
            var service = new DataServiceSubstitute();
            var result = service.Execute();

            // Assert
            Assert.That(result.Count > 0);
        }
    }
}