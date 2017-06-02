using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services DataSplitterServiceUnitTests")]
    public class DataSplitterServiceUnitTests
    {
        [Test]
        public void Execute_SplitSportIntoSectionsForBettypeProducts_ReturnSectionsSuccessfully()
        {
            // Arrange
            var sports = TestLoader.DataLoader<Dictionary<string, DataTable>>("BettypeProducts.json");
            var expected = TestLoader.DataLoader<Dictionary<string, List<DataTable>>>("BettypeProducts-expected.json");
            var dataService = Substitute.For<RawDataService>();
            dataService.Query(Arg.Any<SourceMapping>()).Returns(sports);
            var sourceMapping = new SourceMapping { DataType = DataType.RealWiLoss, SheetName = "Real", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts" };

            // Act
            var service = new DataSplitterServiceImp(dataService);
            var result = service.Execute(sourceMapping);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void Execute_SplitSportIntoSectionsForBettypeProductsJuice_ReturnSectionsSuccessfully()
        {
            // Arrange
            var sports = TestLoader.DataLoader<Dictionary<string, DataTable>>("BettypeProducts-Juice.json");
            var expected = TestLoader.DataLoader<Dictionary<string, List<DataTable>>>("BettypeProducts-Juice-expected.json");
            var dataService = Substitute.For<RawDataService>();
            dataService.Query(Arg.Any<SourceMapping>()).Returns(sports);
            var sourceMapping = new SourceMapping { DataType = DataType.RealWiLoss, SheetName = "Real", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts_Juice" };

            // Act
            var service = new DataSplitterServiceImp(dataService);
            var result = service.Execute(sourceMapping);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}