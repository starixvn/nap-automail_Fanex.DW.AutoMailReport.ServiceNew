using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using Newtonsoft.Json;
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
            var generalData = CreateGeneralData();
            var detailData = CreateDetailData();
            repositoryService.QueryGeneralData(Arg.Any<SourceMapping>()).Returns(generalData);
            repositoryService.QueryDetailData(Arg.Any<SourceMapping>()).Returns(detailData);
            var expected = TestLoader.DataLoader<Dictionary<string, List<DataTable>>>("DataSplitter-Source-expected.json");

            // Act
            var service = new SportSplitterServiceImp(repositoryService);
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
        public void Execute_RemoveRedundantBettypes_ReturnRightData()
        {
            // Arrange
            var repositoryService = Substitute.For<RepositoryBettypeProductService>();
            var generalData = CreateGeneralData();
            var detailData = CreateDetailData();
            repositoryService.QueryGeneralData(Arg.Any<SourceMapping>()).Returns(generalData);
            repositoryService.QueryDetailData(Arg.Any<SourceMapping>()).Returns(detailData);

            var expected = TestLoader.DataLoader<Dictionary<string, List<DataTable>>>("DataSpliter-NumberOfColumnsOnEachSectionIs7-expected.json");

            // Act
            var service = Substitute.ForPartsOf<SportSplitterServiceImp>(new object[] { repositoryService });
            service.NumberOfColumnsOnEachSection.Returns(7);
            var result = service.Execute(new SourceMapping());

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}