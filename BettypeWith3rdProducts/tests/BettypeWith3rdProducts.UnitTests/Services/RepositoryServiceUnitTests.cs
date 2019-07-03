using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services RepositoryServiceUnitTests")]
    public class RepositoryServiceUnitTests
    {
        private ApplicationConfigurationService appConfig;
        private IDynamicRepository dynamicRepository;
        private DateRangeService dateRangeService;
        private DataSourceService dataSourceService;

        private void InitializeSubstitute()
        {
            appConfig = Substitute.For<ApplicationConfigurationService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            dateRangeService = Substitute.For<DateRangeService>();
            dataSourceService = Substitute.For<DataSourceService>();

            dateRangeService.DateStart.Returns(new DateTime(2017, 01, 01));
            dateRangeService.DateEnd.Returns(new DateTime(2017, 01, 31));

            appConfig.IsManual.Returns(false);

            var dataTable = CreateDataTable();
            dataSourceService.Query(Arg.Any<CriteriaBase>()).Returns(dataTable);
        }

        private DataTable CreateDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Date", typeof(DateTime));
            var dataRow = dataTable.NewRow();
            dataRow[0] = new DateTime(2017, 01, 31);
            dataTable.Rows.Add(dataRow);

            return dataTable;
        }

        [Test]
        public void QueryGeneralData_Always_ReturnRightData()
        {
            // Arrange
            InitializeSubstitute();

            var rawDataSource = CreateRawDataSource();
            var expected = CreateGeneralData();

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository);
            var result = service.QueryGeneralData(rawDataSource);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private Dictionary<string, DataTable> CreateRawDataSource()
        {
            var dataTable = CreateDataTable();
            return new Dictionary<string, DataTable>
            {
                { "All", dataTable },
                { "Summary", dataTable },
                { "Soccer", dataTable },
                { "Basketball", dataTable },
                { "Baseball", dataTable }
            };
        }

        private Dictionary<string, DataTable> CreateGeneralData()
        {
            var dataTable = CreateDataTable();
            return new Dictionary<string, DataTable>
            {
                { "All", dataTable },
                { "Summary", dataTable }
            };
        }

        [Test]
        public void QueryDetailData_Always_ReturnRightData()
        {
            // Arrange
            InitializeSubstitute();

            var rawDataSource = CreateRawDataSource();
            var expected = CreateDetailData();

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository);
            var result = service.QueryDetailData(rawDataSource);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void QueryDetailData_DataTableIsEmpty_ReturnRightData()
        {
            // Arrange
            InitializeSubstitute();

            var rawDataSource = new Dictionary<string, DataTable>();
            var expected = 0;

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository);
            var result = service.QueryDetailData(rawDataSource);

            // Assert
            Assert.That(result.Count, Is.EqualTo(expected));
        }

        private Dictionary<string, DataTable> CreateDetailData()
        {
            var dataTable = CreateDataTable();
            return new Dictionary<string, DataTable>
            {
                { "Soccer", dataTable },
                { "Basketball", dataTable },
                { "Baseball", dataTable }
            };
        }

        [Test]
        public void QueryLastDate_Always_ReturnRightValue()
        {
            // Arrange
            InitializeSubstitute();
            dynamicRepository.Get<LastDateModel>(Arg.Any<CriteriaBase>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 01, 01) });
            var expected = new DateTime(2017, 01, 01);

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository);
            var result = service.QueryLastDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsDataAvailable_LastDateLargerThanYesterday_ReturnTrue()
        {
            // Arrange
            InitializeSubstitute();
            SystemTime.Set(new DateTime(2017, 01, 02));
            dynamicRepository.Get<LastDateModel>(Arg.Any<DWStatusByCubeCriteria>()).Returns(new LastDateModel { LastDate = new DateTime(2017, 01, 01) });
            appConfig.IsCheckEnoughData.Returns(true);

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository);
            var result = service.IsDataAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }
    }
}