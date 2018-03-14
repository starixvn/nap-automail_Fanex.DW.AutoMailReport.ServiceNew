using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services SportSplitterServiceUnitTests")]
    public class SportSplitterServiceUnitTests
    {
        [Test]
        public void Execute_SplitSportIntoManySections_ReturnRightData()
        {
            // Arrange
            var repositoryService = Substitute.For<RepositoryBettypeProductService>();
            var rawDataSourceService = Substitute.For<RawDataSourceService>();
            var generalData = CreateGeneralData();
            var detailData = CreateDetailData();
            repositoryService.QueryGeneralData(Arg.Any<Dictionary<string, DataTable>>()).Returns(generalData);
            repositoryService.QueryDetailData(Arg.Any<Dictionary<string, DataTable>>()).Returns(detailData);
            var expected = TestLoader.DataLoader<Dictionary<string, List<DataTable>>>("DataSplitter-Source-expected.json");

            // Act
            var service = new SportSplitterServiceImp(repositoryService, rawDataSourceService);
            var result = service.Execute(new SourceMapping());

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private Dictionary<string, DataTable> CreateGeneralData()
        {
            var dataTable = TestLoader.DataLoader<DataTable>("DataSplitter-Source.json");
            return new Dictionary<string, DataTable>
            {
                { "All", dataTable },
                { "Summary", dataTable }
            };
        }

        private Dictionary<string, DataTable> CreateDetailData()
        {
            var dataTable = TestLoader.DataLoader<DataTable>("DataSplitter-Source.json");
            return new Dictionary<string, DataTable>
            {
                { "Soccer", dataTable }
            };
        }

        [Test]
        public void GetNumberOfSections()
        {
            var columnCount = 210;
            var numberOfSections = columnCount / 70;
            numberOfSections = columnCount % 70 > 0
                ? numberOfSections + 1
                : numberOfSections;

            Assert.That(numberOfSections, Is.EqualTo(3));
        }
    }
}