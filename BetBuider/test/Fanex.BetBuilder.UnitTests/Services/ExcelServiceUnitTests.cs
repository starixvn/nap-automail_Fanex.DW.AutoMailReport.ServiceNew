using System.Collections.Generic;
using System.IO;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using OfficeOpenXml;
using SimpleInjector;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            Container container = new Container();
            RegisterService.RegisterContainer(container);
            var text = File.ReadAllText("Templates/MonthlyDataModel.json");
            var data = JsonConvert.DeserializeObject<List<MonthlyDataModel>>(text);

            var excelPackage = container.GetInstance<ExcelPackage>();
            var dateTimeService = container.GetInstance<DateTimeService>();

            // Act
            var service = new ExcelServiceImp(excelPackage, dateTimeService);
            var path = service.Execute(data);

            // Assert
            Assert.That(path.Contains(".xlsx"));
        }
    }
}