using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Utility;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface DataService
    {
        Dictionary<string, Dictionary<string, List<DataTable>>> Execute();

        bool IsDataAvailable();
    }

    public class DataServiceImp : DataService
    {
        private readonly ApplicationConfigurationService appConfig;
        private readonly DataSplitterService dataSplitterService;
        private readonly DataSourceService dataSourceService;

        public DataServiceImp(ApplicationConfigurationService appConfig, DataSplitterService dataSplitterService, DataSourceService dataSourceService)
        {
            this.appConfig = appConfig;
            this.dataSplitterService = dataSplitterService;
            this.dataSourceService = dataSourceService;
        }

        public Dictionary<string, Dictionary<string, List<DataTable>>> Execute()
        {
            var dataSource = new Dictionary<string, Dictionary<string, List<DataTable>>>();
            var sourceMappings = appConfig.SourceMappings;
            foreach (var sourceMapping in sourceMappings)
            {
                dataSource.Add(sourceMapping.SheetName, dataSplitterService.Execute(sourceMapping));
            }

            return dataSource;
        }

        public bool IsDataAvailable()
        {
            var result = false;
            if (appConfig.ReportType == DateRange.Manual || !appConfig.IsCheckEnoughData)
            {
                return true;
            }

            result = dataSourceService.QueryLastDate() >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }
    }
}