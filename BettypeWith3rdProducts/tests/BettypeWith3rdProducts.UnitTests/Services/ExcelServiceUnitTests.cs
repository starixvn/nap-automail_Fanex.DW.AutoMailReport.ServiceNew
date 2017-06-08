using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NSubstitute;
using NUnit.Framework;
using OfficeOpenXml;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        private ExcelPackage excelPackage;
        private ApplicationConfigurationService appConfig;
        private DateRangeServiceProvider dateRangeServiceProvider;
        private ExcelHeaderService excelHeaderService;
        private DataService dataService;

        private void InitializeSubstitute()
        {
            var fileName = $@"{TestContext.CurrentContext.TestDirectory}\Templates\Template.xlsx";
            excelPackage = new ExcelPackage(new System.IO.FileInfo(fileName));
            appConfig = Substitute.For<ApplicationConfigurationService>();
            dateRangeServiceProvider = Substitute.For<DateRangeServiceProvider>();
            excelHeaderService = new ExcelHeaderServiceImp();
            dataService = Substitute.For<DataService>();
            var dateRangeService = Substitute.For<DateRangeService>();
            dateRangeService.DateStart.Returns(new DateTime(2017, 01, 01));
            dateRangeService.DateEnd.Returns(new DateTime(2017, 01, 31));
            dateRangeServiceProvider.Date(Arg.Any<DateRange>()).Returns(dateRangeService);
            appConfig.ExportDataDirectory.Returns(@"\\10.70.70.242\www\Olympus\Fanex.Excel\BettypeWith3rdProducts");

            var dataSource = CreateDataSource();
            dataService.Execute().Returns(dataSource);
        }

        [Test]
        public void ExcelService_Always_ExportSuccessfully()
        {
            // Arrange
            InitializeSubstitute();

            // Act
            var service = new ExcelServiceImp(excelPackage, appConfig, dataService, dateRangeServiceProvider, excelHeaderService);
            var result = service.Execute();

            // Assert
            Assert.That(File.Exists(result));
        }

        [Test]
        public void ExcelService_Always_NumberOfSheetsAre4()
        {
            // Arrange
            InitializeSubstitute();

            // Act
            var service = new ExcelServiceImp(excelPackage, appConfig, dataService, dateRangeServiceProvider, excelHeaderService);
            var result = service.Execute();

            // Assert
            Assert.That(excelPackage.Workbook.Worksheets.Count == 4);
        }

        [Test]
        public void ExcelService_Always_RenderRightDateRange()
        {
            // Arrange
            InitializeSubstitute();
            var expected = "MIS_Bettype with 3rd products From 01/01/2017 To 01/31/2017";

            // Act
            var service = new ExcelServiceImp(excelPackage, appConfig, dataService, dateRangeServiceProvider, excelHeaderService);
            service.Execute();
            var sheet = excelPackage.Workbook.Worksheets[1];
            var result = sheet.Cells["B2"].Value.ToString();

            // Assert
            Assert.That(result.Equals(expected));
        }

        private Dictionary<string, Dictionary<string, List<DataTable>>> CreateDataSource()
        {
            var dataTable = CreateDataTable();
            return new Dictionary<string, Dictionary<string, List<DataTable>>>
            {
                { "Real", new Dictionary<string, List<DataTable>> { { "All", new List<DataTable> { dataTable } } } },
                { "Virtual", new Dictionary<string, List<DataTable>> { { "All", new List<DataTable> { dataTable } } } },
                { "Real Juice & Leftover", new Dictionary<string, List<DataTable>> { { "All", new List<DataTable> { dataTable } } } },
                { "Virtual Juice & Leftover", new Dictionary<string, List<DataTable>> { { "All", new List<DataTable> { dataTable } } } }
            };
        }

        private DataTable CreateDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("HDP@#@TO");
            dataTable.Columns.Add("HDP@#@WL");

            return dataTable;
        }
    }
}