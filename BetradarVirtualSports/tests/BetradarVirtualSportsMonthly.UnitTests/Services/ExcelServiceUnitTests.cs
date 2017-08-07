using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;
using OfficeOpenXml;

namespace BetradarVirtualSportsMonthly.UnitTests.Services
{
    [Category("BetradarVirtualSportsMonthly.UnitTests.Services ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        private readonly string excelPath = @"Templates\Monthly-Template.xlsx";

        private DataService dataService;
        private ApplicationConfigurationService appConfigService;
        private DateService dateService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            dataService = Substitute.For<DataService>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            dateService = Substitute.For<DateService>();
            dateService.FromDate.Returns(new DateTime(2017, 06, 01));
            dateService.ToDate.Returns(new DateTime(2017, 06, 30));
        }

        [Test]
        public void Export_Nodata_MessageNoDataShouldBeDisplayed()
        {
            // Arrange
            var excelPackage = new ExcelPackage(new FileInfo(excelPath));

            dataService.QueryMonthly(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(new Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>>(new List<GrossWinningMonthly>(), new List<GrossWinningMonthly>()));
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));

            var expected = "There is no data displayed";

            // Act
            var service = new ExcelServiceImp(excelPackage, dataService, appConfigService, dateService);
            service.Export();
            var sheet = excelPackage.Workbook.Worksheets["BetradarVS_Monthly"];
            var result = sheet.Cells["A4"].Value;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Export_SetManualIsTrue_Successfully()
        {
            // Arrange
            var excelPackage = new ExcelPackage(new FileInfo(excelPath));

            var rawData = TestLoader.DataLoader<Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>>>("QueryMonthly.json");
            dataService.QueryMonthly(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(rawData);
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            var expected = 10;

            // Act
            var service = new ExcelServiceImp(excelPackage, dataService, appConfigService, dateService);
            service.Export().Save();
            var sheet = excelPackage.Workbook.Worksheets["BetradarVS_Monthly"];
            var result = sheet.Dimension.End.Row;

            // Assert
            Assert.GreaterOrEqual(result, expected);
        }

        [Test]
        public void Export_SetManualIsFalse_Successfully()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 07, 07));
            var excelPackage = new ExcelPackage(new FileInfo(excelPath));

            var rawData = TestLoader.DataLoader<Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>>>("QueryMonthly.json");
            dataService.QueryMonthly(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(rawData);
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            var expected = 10;

            // Act
            var service = new ExcelServiceImp(excelPackage, dataService, appConfigService, dateService);
            service.Export().Save();
            var sheet = excelPackage.Workbook.Worksheets["BetradarVS_Monthly"];
            var result = sheet.Dimension.End.Row;

            // Assert
            Assert.GreaterOrEqual(result, expected);
        }
    }
}