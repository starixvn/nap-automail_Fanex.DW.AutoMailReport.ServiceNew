using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleInjector;

namespace BettypeWith3rdProducts.IntegrationTests.Services
{
    [Category("BettypeWith3rdProducts.IntegrationTests.Services RawDataServiceTests")]
    public class RawDataServiceTests
    {
        private DataService service;

        [OneTimeSetUp]
        public void Initialize()
        {
            FanexDataRegisterService.ConfigureFanexData();
            var container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            service = container.GetInstance<DataService>();
        }

        [Test]
        public void Query_Always_ConnectToDataBaseSuccessfully()
        {
            // Act
            var result = service.Execute();
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            // Assert
            Assert.That(result.Count > 0);
        }

        [Test]
        public void Test()
        {
            var data = TestLoader.DataLoader<Dictionary<string, Dictionary<string, List<DataTable>>>>("BettypeProducts-All.json");
        }
    }
}