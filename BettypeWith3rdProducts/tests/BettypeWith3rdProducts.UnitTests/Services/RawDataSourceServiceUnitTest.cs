using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services.RawDataSourceServiceUnitTest")]
    public class RawDataSourceServiceUnitTest
    {
        private DataSourceService dataSourceService;
        private DateRangeService dateRangeService;

        [SetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            dataSourceService = Substitute.For<DataSourceService>();
            dateRangeService = Substitute.For<DateRangeService>();
        }

        [Test]
        public void QueryDataSouce_Execute_ReturnRightData()
        {
            // Arrange
            var text = File.ReadAllText(@"Templates\SourceMapping.json");
            var sourceMapping = JsonConvert.DeserializeObject<List<SourceMapping>>(text).FirstOrDefault();
            text = File.ReadAllText(@"Templates\QueryDataSouce-RawData.json");
            var data = JsonConvert.DeserializeObject<DataSet>(text);
            dataSourceService.QueryMultiple(Arg.Any<CriteriaBase>()).Returns(data);
            text = File.ReadAllText(@"Templates\QueryDataSouce-ExpectedData.json");
            var expected = JsonConvert.DeserializeObject<Dictionary<string, DataTable>>(text);

            // Act
            var service = new RawDataSourceServiceImp(dateRangeService, dataSourceService);
            var result = service.QueryDataSouce(sourceMapping);            

            // Assert            
            Assert.That(expected, NUnit.DeepObjectCompare.Is.DeepEqualTo(result));
        }
    }
}