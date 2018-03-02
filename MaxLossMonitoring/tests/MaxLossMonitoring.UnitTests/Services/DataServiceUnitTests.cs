using System;
using System.Collections.Generic;
using System.IO;
using MaxLossMonitoring.Business.Models;
using MaxLossMonitoring.Business.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace MaxLossMonitoring.UnitTests.Services
{
    [Category("MaxLossMonitoring.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void QueryLevel_Always_ReturnRightValue()
        {
            // Arrange
            var repository = Substitute.For<MaxLossRepositoryService>();
            repository.QueryLevel().Returns(JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText("Templates/LevelRawData.json")));
            var expected = JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText("Templates/ExpectedLevel.json"));

            // Action
            var service = new DataServiceImp(repository);
            var result = service.QueryLevel();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void QueryMonitorListForLetstalk_Always_ReturnRightValue()
        {
            // Arrange
            var repository = Substitute.For<MaxLossRepositoryService>();
            repository.QueryLevel().Returns(JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText("Templates/LevelRawData.json")));
            repository.QueryMonitorListForLetstalk().Returns(JsonConvert.DeserializeObject<List<MonitorListRaw>>(File.ReadAllText("Templates/QueryMonitorListForLetstalkRawData.json")));
            var expected = JsonConvert.DeserializeObject<List<MonitorList>>(File.ReadAllText("Templates/ExpectedQueryMonitorListForLetstalk.json"));

            // Action
            var service = new DataServiceImp(repository);
            var result = service.QueryMonitorListForLetstalk();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void QueryMonitorListForEmail_Always_ReturnRightValue()
        {
            // Arrange
            var repository = Substitute.For<MaxLossRepositoryService>();
            repository.QueryLevel().Returns(JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText("Templates/LevelRawData.json")));
            repository.QueryMonitorListForEmail().Returns(JsonConvert.DeserializeObject<Tuple<IEnumerable<MonitorListRaw>, IEnumerable<MonitorListRaw>>>(File.ReadAllText("Templates/QueryMonitorListForEmailRawData.json")));
            var expected = JsonConvert.DeserializeObject<Tuple<List<MonitorList>, List<MonitorListRaw>>>(File.ReadAllText("Templates/ExpectedQueryMonitorListForEmail.json"));

            // Action
            var service = new DataServiceImp(repository);
            var result = service.QueryMonitorListForEmail();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}