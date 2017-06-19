using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
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
        private DateRangeServiceProvider dateRangeServiceProvider;
        private ParameterService parameterService;
        private DataSourceService dataSourceService;

        private void InitializeSubstitute()
        {
            appConfig = Substitute.For<ApplicationConfigurationService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            dateRangeServiceProvider = Substitute.For<DateRangeServiceProvider>();
            parameterService = new ParameterServiceImp();
            dataSourceService = Substitute.For<DataSourceService>();

            var dateRangeService = Substitute.For<DateRangeService>();
            dateRangeService.DateStart.Returns(new DateTime(2017, 01, 01));
            dateRangeService.DateEnd.Returns(new DateTime(2017, 01, 31));

            dateRangeServiceProvider.Date(DateRange.Daily).Returns(dateRangeService);
            appConfig.ReportType.Returns(DateRange.Daily);

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

            var expected = CreateGeneralData();

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository, dateRangeServiceProvider, parameterService, dataSourceService);
            var result = service.QueryGeneralData(new SourceMapping());

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
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
            dynamicRepository.Fetch<SportType>(Arg.Any<CriteriaBase>()).Returns(new List<SportType>
            {
                new SportType { Sport = "Soccer", SportId = 1 },
                new SportType { Sport = "Basketball", SportId = 2 },
                new SportType { Sport = "Football", SportId = 3 }
            });

            var expected = CreateDetailData();

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository, dateRangeServiceProvider, parameterService, dataSourceService);
            var result = service.QueryDetailData(new SourceMapping());

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void QueryDetailData_DuplicatedSport_ReturnRightData()
        {
            // Arrange
            InitializeSubstitute();
            dynamicRepository.Fetch<SportType>(Arg.Any<CriteriaBase>()).Returns(new List<SportType>
            {
                new SportType { Sport = "Soccer", SportId = 1 },
                new SportType { Sport = "Soccer", SportId = 1 },
                new SportType { Sport = "Basketball", SportId = 2 },
                new SportType { Sport = "Football", SportId = 3 }
            });

            var expected = CreateDetailData();

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository, dateRangeServiceProvider, parameterService, dataSourceService);
            var result = service.QueryDetailData(new SourceMapping());

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        [Test]
        public void QueryDetailData_DataTableIsEmpty_ReturnRightData()
        {
            // Arrange
            InitializeSubstitute();
            dataSourceService.Query(Arg.Any<CriteriaBase>()).Returns(new DataTable());
            dynamicRepository.Fetch<SportType>(Arg.Any<CriteriaBase>()).Returns(new List<SportType>
            {
                new SportType { Sport = "Soccer", SportId = 1 }
            });

            var expected = 0;

            // Act
            var service = new RepositoryServiceImp(appConfig, dynamicRepository, dateRangeServiceProvider, parameterService, dataSourceService);
            var result = service.QueryDetailData(new SourceMapping());

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
                { "Football", dataTable },
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
            var service = new RepositoryServiceImp(appConfig, dynamicRepository, dateRangeServiceProvider, parameterService, dataSourceService);
            var result = service.QueryLastDate();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}