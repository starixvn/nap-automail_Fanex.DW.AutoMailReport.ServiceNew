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
        }

        [Test]
        public void ExcelService_Always_ExportSuccessfully()
        {
            // Arrange
            InitializeSubstitute();
            var dataSource = CreateDataSource();
            dataService.Execute().Returns(dataSource);          

            // Act
            var service = new ExcelServiceImp(excelPackage, appConfig, dataService, dateRangeServiceProvider, excelHeaderService);
            var result = service.Execute();

            // Assert
            Assert.That(File.Exists(result));
            Assert.That(excelPackage.Workbook.Worksheets.Count == 4);
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