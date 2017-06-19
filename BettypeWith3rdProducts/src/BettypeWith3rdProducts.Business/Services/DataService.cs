using System;
using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using Fanex.Service.Core;

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
        private readonly SportSplitterService sportSplitterService;
        private readonly RepositoryService repositoryService;

        public DataServiceImp(ApplicationConfigurationService appConfig, SportSplitterService sportSplitterService, RepositoryService repositoryService)
        {
            this.appConfig = appConfig;
            this.sportSplitterService = sportSplitterService;
            this.repositoryService = repositoryService;
        }

        public Dictionary<string, Dictionary<string, List<DataTable>>> Execute()
        {
            var dataSource = new Dictionary<string, Dictionary<string, List<DataTable>>>();
            var sourceMappings = appConfig.SourceMappings;
            foreach (var sourceMapping in sourceMappings)
            {
                dataSource.Add(sourceMapping.SheetName, sportSplitterService.Execute(sourceMapping));
            }

            return dataSource;
        }

        public bool IsDataAvailable()
        {
            if (appConfig.ReportType == DateRange.Manual || !appConfig.IsCheckEnoughData)
            {
                return true;
            }

            var result = repositoryService.QueryLastDate() >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }
    }
}