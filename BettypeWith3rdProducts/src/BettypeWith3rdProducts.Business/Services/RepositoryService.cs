using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface RepositoryService
    {
        Dictionary<string, DataTable> QueryGeneralData(SourceMapping sourceMapping);

        Dictionary<string, DataTable> QueryDetailData(SourceMapping sourceMapping);

        DateTime QueryLastDate();
    }

    public class RepositoryServiceImp : RepositoryService
    {
        private readonly ApplicationConfigurationService appConfig;
        private readonly IDynamicRepository dynamicRepository;
        private readonly DateRangeServiceProvider dateRangeService;
        private readonly ParameterService parameterService;
        private readonly DataSourceService dataSourceService;

        public RepositoryServiceImp(
            ApplicationConfigurationService appConfig,
            IDynamicRepository dynamicRepository,
            DateRangeServiceProvider dateRangeService,
            ParameterService parameterService,
            DataSourceService dataSourceService)
        {
            this.appConfig = appConfig;
            this.dynamicRepository = dynamicRepository;
            this.dateRangeService = dateRangeService;
            this.parameterService = parameterService;
            this.dataSourceService = dataSourceService;
        }

        public Dictionary<string, DataTable> QueryGeneralData(SourceMapping sourceMapping)
        {
            var dataSource = new Dictionary<string, DataTable>();
            QueryDataByViewOfData(ViewOfData.All, sourceMapping, dataSource);
            QueryDataByViewOfData(ViewOfData.Summary, sourceMapping, dataSource);

            return dataSource;
        }

        public Dictionary<string, DataTable> QueryDetailData(SourceMapping sourceMapping)
        {
            var dataSource = new Dictionary<string, DataTable>();
            var sports = QuerySports(sourceMapping.DataType);
            foreach (var sport in sports)
            {
                if (dataSource.ContainsKey(sport.Sport))
                {
                    continue;
                }

                var dataTable = QueryDataSouce(sourceMapping, appConfig.ReportType, ViewOfData.Detail, sport.SportId);
                if (dataTable.Columns.Count > 0 && dataTable.Rows.Count > 0)
                {
                    dataSource.Add(sport.Sport, dataTable);
                }
            }

            return dataSource;
        }

        private void QueryDataByViewOfData(ViewOfData viewOfData, SourceMapping sourceMapping, Dictionary<string, DataTable> dataSource)
        {
            var dataTable = QueryDataSouce(sourceMapping, appConfig.ReportType, viewOfData, 0);
            dataSource.Add(viewOfData.ToString(), dataTable);
        }

        private DataTable QueryDataSouce(SourceMapping sourceMapping, DateRange dateRange, ViewOfData viewOfData, int sportId)
        {
            var date = dateRangeService.Date(dateRange);
            var parameter = parameterService.GetParameter(viewOfData, sportId);
            var criteria = new BettypeProductCriteria
            {
                DateStart = date.DateStart,
                DateEnd = date.DateEnd,
                Sportid = parameter.SportId,
                Periodid = parameter.PeriodId,
                StatisticId = (int)sourceMapping.DataType,
                StoreProcedure = sourceMapping.StoreProcedure
            };

            return dataSourceService.Query(criteria);
        }

        private IEnumerable<SportType> QuerySports(DataType dataType)
        {
            return dynamicRepository.Fetch<SportType>(new SportCriteria { Include3Rd = dataType == DataType.RealWiLoss });
        }

        public DateTime QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new DWStatusByCubeCriteria()).LastDate;
        }
    }
}