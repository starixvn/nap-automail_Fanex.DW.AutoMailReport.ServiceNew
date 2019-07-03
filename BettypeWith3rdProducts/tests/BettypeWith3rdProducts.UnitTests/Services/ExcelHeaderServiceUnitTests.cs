using System.Data;
using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;
using OfficeOpenXml;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ExcelHeaderServiceUnitTests")]
    public class ExcelHeaderServiceUnitTests
    {
        [Test]
        public void SplitHeader_Always_Successfully()
        {
            // Arrange
            var excelPackage = new ExcelPackage();
            var workSheet = excelPackage.Workbook.Worksheets.Add("Real");
            var section = CreateDataSource();
            var startRow = 2;

            // Act
            var service = new ExcelHeaderServiceImp();
            service.SplitHeader(workSheet, section, startRow);

            // Assert
            Assert.That(workSheet.Cells[2, 2].Value.ToString() == "Date");
            Assert.That(workSheet.Cells[2, 3].Value.ToString() == "HDP");
            Assert.That(workSheet.Cells[3, 3].Value.ToString() == "TO");
            Assert.That(workSheet.Cells[3, 4].Value.ToString() == "WL");

            Assert.That(workSheet.Cells[2, 2, 3, 2].Merge);
            Assert.That(workSheet.Cells[2, 3, 2, 4].Merge);
        }

        private DataTable CreateDataSource()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("HDP@#@TO");
            dataTable.Columns.Add("HDP@#@WL");

            return dataTable;
        }
    }
}