using System;
using System.Collections.Generic;
using System.IO;
using Fanex.BetBuilder.Business.Criterias;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        [Test]
        public void QueryDailyData_Awlays_ReturnRightData()
        {
            // Arrange
            var param = Substitute.For<CriteriaService>();
            param.WinlostDate.Returns(new DateTime(2019, 05, 01));
            var repository = Substitute.For<IDynamicRepository>();
            var text = File.ReadAllText("Templates/DailyRawDataModel.json");
            var expected = JsonConvert.DeserializeObject<List<DailyRawDataModel>>(text);
            repository.Fetch<DailyRawDataModel>(Arg.Compat.Any<DailyBetBuilderCriteria>()).Returns(expected);

            // Act
            var service = new DataServiceImp(repository, param);
            var result = service.QueryDailyData();
            
            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }
    }
}