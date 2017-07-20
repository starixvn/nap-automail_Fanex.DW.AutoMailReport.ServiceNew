using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSports.Business.Models;
using BetradarVirtualSports.Business.Services;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.Framework;
using OfficeOpenXml;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        private DataDailyService dataDailyService;
        private ApplicationConfigurationService appConfigService;
        private DateService dateService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            dataDailyService = Substitute.For<DataDailyService>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            dateService = Substitute.For<DateService>();
        }

        [Test]
        public void Export_Nodata_MessageNoDataShouldBeDisplayed()
        {
            // Arrange
            var excelPackage = new ExcelPackage(new FileInfo(@"Templates\Daily-Template.xlsx"));

            dataDailyService.QueryDaily(Arg.Any<DateTime>()).Returns(new Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>>(new List<GrossWinningDaily>(), new List<GrossWinningDaily>()));
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));

            var expected = "There is no data displayed";

            // Act
            var service = new ExcelServiceImp(excelPackage, dataDailyService, appConfigService, dateService);
            service.Export();
            var sheet = excelPackage.Workbook.Worksheets["BetradarVS_Daily"];
            var result = sheet.Cells["A4"].Value;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Export_SetManualIsTrue_Successfully()
        {
            // Arrange
            var excelPackage = new ExcelPackage(new FileInfo(@"Templates\Daily-Template.xlsx"));

            var rawData = TestLoader.DataLoader<Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>>>("QueryDaily.json");
            dataDailyService.QueryDaily(Arg.Any<DateTime>()).Returns(rawData);
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            var expected = 10;

            // Act
            var service = new ExcelServiceImp(excelPackage, dataDailyService, appConfigService, dateService);
            service.Export().Save();
            var sheet = excelPackage.Workbook.Worksheets["BetradarVS_Daily"];
            var result = sheet.Dimension.End.Row;

            // Assert
            Assert.GreaterOrEqual(result, expected);
        }

        [Test]
        public void Export_SetManualIsFalse_Successfully()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 07, 07));
            var excelPackage = new ExcelPackage(new FileInfo(@"Templates\Daily-Template.xlsx"));

            var rawData = TestLoader.DataLoader<Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>>>("QueryDaily.json");
            dataDailyService.QueryDaily(Arg.Any<DateTime>()).Returns(rawData);
            appConfigService.Configurations.Returns(TestLoader.DataLoader<AppConfig>("AppConfig.json"));
            appConfigService.Configurations.IsManual = false;
            var expected = 10;

            // Act
            var service = new ExcelServiceImp(excelPackage, dataDailyService, appConfigService, dateService);
            service.Export().Save();
            var sheet = excelPackage.Workbook.Worksheets["BetradarVS_Daily"];
            var result = sheet.Dimension.End.Row;

            // Assert
            Assert.GreaterOrEqual(result, expected);
        }
    }
}