using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using BettypeWith3rdProducts.Business.Utility;
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
            appConfig.ReportType.Returns(DateRange.Manual);
            appConfig.IsCheckEnoughData.Returns(true);
            var expected = true;

            // Act
            var service = new DataServiceImp(appConfig, sportSplitterService, repositoryService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result == expected);
        }

        [Test]
        public void IsDataAvailable_QueryLastDateLargerThanSystemTime_AlwaysReturnTrue()
        {
            // Arrange
            InitializeSubstitute();
            appConfig.IsCheckEnoughData.Returns(false);
            appConfig.ReportType.Returns(DateRange.Daily);
            var expected = true;

            // Act
            var service = new DataServiceImp(appConfig, sportSplitterService, repositoryService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result == expected);
        }

        [Test]
        public void IsDataAvailable_IsNotCheckEnoughData_AlwaysReturnTrue()
        {
            // Arrange
            InitializeSubstitute();
            appConfig.IsCheckEnoughData.Returns(true);
            appConfig.ReportType.Returns(DateRange.Daily);
            SystemTime.Set(new DateTime(2017, 01, 02));
            repositoryService.QueryLastDate().Returns(new DateTime(2017, 01, 01));
            var expected = true;

            // Act
            var service = new DataServiceImp(appConfig, sportSplitterService, repositoryService);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result == expected);
        }
    }
}