using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services RawDataServiceUnitTests")]
    public class RawDataServiceUnitTests
    {
        [Test]
        public void Query_Always_ReturnRightData()
        {
            // Arrange
            var appConfig = Substitute.For<ApplicationConfigurationService>();
            var sourceMapping = new SourceMapping { DataType = DataType.RealWiLoss, SheetName = "Real", StoreProcedure = "SPName" };

            var dataSourceService = Substitute.For<DataSourceService>();
            var dataSource = CreateDataSource();
            dataSourceService.QueryDataSouce(Arg.Any<SourceMapping>(), Arg.Any<DateRange>(), Arg.Any<ViewOfData>(), Arg.Any<int>()).Returns(dataSource);
            dataSourceService.QuerySports(Arg.Any<DataType>()).Returns(new List<SportType> { new SportType { SportId = 1, Sport = "Soccer" } });
            var expected = CreateExpected();

            // Act
            var service = new RawDataServiceImp(dataSourceService, appConfig);
            var result = service.Query(sourceMapping);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private DataTable CreateDataSource()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Date", typeof(DateTime));
            var dataRow = dataTable.NewRow();
            dataRow["Date"] = new DateTime(2017, 01, 01);
            dataTable.Rows.Add(dataRow);

            return dataTable;
        }

        private Dictionary<string, DataTable> CreateExpected()
        {
            var dataSource = CreateDataSource();
            return new Dictionary<string, DataTable>
            {
                { "All", dataSource },
                { "Summary", dataSource },
                { "Soccer", dataSource },
            };
        }
    }
}