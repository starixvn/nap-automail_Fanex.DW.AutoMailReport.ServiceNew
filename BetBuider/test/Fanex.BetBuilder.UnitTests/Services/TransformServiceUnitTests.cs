using System.Collections.Generic;
using System.IO;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleInjector;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.TransformServiceUnitTests")]
    public class TransformServiceUnitTests
    {
        private Container container;

        [SetUp]
        public void Init()
        {
            container = new Container();
            RegisterService.RegisterServices();
            RegisterService.RegisterContainer(container);
        }

        [Test]
        public void ExecuteDaily_Always_ReturnRightData()
        {
            // Arrange
            var text = File.ReadAllText("Templates/DailyRawDataModel.json");
            var rawData = JsonConvert.DeserializeObject<List<DailyRawDataModel>>(text);
            text = File.ReadAllText("Templates/DailyDataModel.json");
            var expected = JsonConvert.DeserializeObject<List<DailyDataModel>>(text);

            // Act
            var service = container.GetInstance<TransformService>();
            var result = service.ExecuteDaily(rawData);

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }

        [Test]
        public void ExecuteMonthly_Always_ReturnRightData()
        {
            // Arrange
            var text = File.ReadAllText("Templates/MonthlyRawDataModel.json");
            var rawData = JsonConvert.DeserializeObject<List<MonthlyRawDataModel>>(text);
            text = File.ReadAllText("Templates/MonthlyDataModel.json");
            var expected = JsonConvert.DeserializeObject<List<MonthlyDataModel>>(text);

            // Act
            var service = container.GetInstance<TransformService>();
            var result = service.ExecuteMonthly(rawData);

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }
    }
}