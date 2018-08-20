using System;
using System.Data;
using System.IO;
using EventMarketOffer.Business.Models;
using EventMarketOffer.Business.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using OfficeOpenXml;

namespace EventMarketOffer.UnitTests.Services
{
    [Category("EventMarketOffer.UnitTests.Services ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        private ExcelPackage excelPackage;
        private AppConfigurationService appConfigService;
        private DateService dateService;
        private DataService dataService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            excelPackage = new ExcelPackage(new FileInfo(@"Templates\Template.xlsx"));
            appConfigService = Substitute.For<AppConfigurationService>();
            dateService = Substitute.For<DateService>();
            dataService = Substitute.For<DataService>();
        }

        [Test]
        public void Export_Always_MethodQueryShouldBeCalled()
        {
            // Arrange
            var text = File.ReadAllText(@"Templates\DataSource.json");
            var dataTable = JsonConvert.DeserializeObject<DataTable>(text);
            dataService.Query().Returns(dataTable);

            // Act
            var service = new ExcelServiceImp(excelPackage, appConfigService, dateService, dataService);
            service.Export();

            // Assert
            dataService.Received().Query();
        }

        [Test]
        public void Save_Always_Successfully()
        {
            // Arrange            
            dateService.DateStart.Returns(new DateTime(2017, 07, 01));
            dateService.DateEnd.Returns(new DateTime(2017, 07, 30));
            appConfigService.Configurations.Returns(new AppConfig { ExportDataDirectory = "\\\\10.18.200.120\\www\\Olympus\\Fanex.Excel\\EventMarketOffer", ExcelPasswordProtected = string.Empty });
            var expected = @"\\10.18.200.120\www\Olympus\Fanex.Excel\EventMarketOffer\2017\Event_Market_Offered_From_01_Jul_2017_To_30_Jul_2017.xlsx";

            // Act
            var service = new ExcelServiceImp(excelPackage, appConfigService, dateService, dataService);
            var result = service.Save();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}