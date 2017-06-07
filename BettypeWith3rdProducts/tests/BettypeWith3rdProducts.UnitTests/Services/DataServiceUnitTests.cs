using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        [Test]
        public void Execute_Always_ReturnRightData()
        {
            // Arrange
            var sourceMappings = TestLoader.DataLoader<List<SourceMapping>>("SourceMapping.json");
            var appConfig = Substitute.For<ApplicationConfigurationService>();
            appConfig.SourceMappings.Returns(sourceMappings);

            var dataSource = CreateDataSource();
            var dataSplitterService = Substitute.For<SportSplitterService>();
            dataSplitterService.Execute(Arg.Any<SourceMapping>()).Returns(dataSource);
            var repositoryService = Substitute.For<RepositoryService>();

            var expected = CreateExpected();

            // Act
            var service = new DataServiceImp(appConfig, dataSplitterService, repositoryService);
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
    }
}