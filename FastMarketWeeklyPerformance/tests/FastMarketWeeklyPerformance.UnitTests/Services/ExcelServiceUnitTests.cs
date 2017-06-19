using System;
using System.Data;
using System.IO;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Repositories;
using FastMarketWeeklyPerformance.Business.Services;
using NSubstitute;
using NUnit.Framework;
using OfficeOpenXml;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        private DataSourceService dataSourceService;
        private DateRangeService dateRangeService;
        private ApplicationConfigurationService appConfig;
        private ExcelPackage excelPackage;
        private DateRange dateRange;

        private void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            dataSourceService = Substitute.For<DataSourceService>();
            dateRangeService = Substitute.For<DateRangeService>();
            appConfig = Substitute.For<ApplicationConfigurationService>();
            excelPackage = new ExcelPackage(new FileInfo(@"Templates\Template.xlsx"));

            dateRange = Substitute.For<DateRange>();

            SystemTime.Set(new DateTime(2017, 01, 08));
            var dataSource = TestLoader.DataLoader<DataTable>("DataSource.json");
            dataSourceService.Query(Arg.Any<FastMarketByWeeklyCriteria>()).Returns(dataSource);
            dateRange.FromDate.Returns(new DateTime(2017, 01, 01));
            dateRange.ToDate.Returns(new DateTime(2017, 01, 07));
            dateRangeService.DateRange(Arg.Any<ReportDateRange>()).Returns(dateRange);
        }

        [Test]
        public void Export_Always_Successfully()
        {
            // Arrange
            Initialize();
            var titleExpected = "Fast Market - Weekly Performance from 01/01/02017 to 07/01/02017";
            appConfig.Configurations.Returns(new AppConfig());

            // Act
            var service = new ExcelServiceImp(dataSourceService, dateRangeService, appConfig, excelPackage);
            service.Export();
            var excelWorksheet = excelPackage.Workbook.Worksheets["Fast Market"];
            var titleResult = excelWorksheet.Cells["B2"].Value;

            // Assert
            Assert.That(titleResult, Is.EqualTo(titleExpected));
        }

        [Test]
        public void Save_Always_Successfully()
        {
            // Arrange
            Initialize();
            var expected = @"\2017\01\20170101_20170107_Automail_FastMarket_WeeklyPerformance.xlsx";
            appConfig.Configurations.Returns(new AppConfig());

            // Act
            var service = new ExcelServiceImp(dataSourceService, dateRangeService, appConfig, excelPackage);
            var result = service.Export().Save();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}