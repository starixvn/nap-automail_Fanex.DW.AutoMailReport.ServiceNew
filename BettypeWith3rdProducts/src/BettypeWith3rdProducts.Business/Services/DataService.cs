using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Data;

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
        private readonly IDynamicRepository repository;

        public DataServiceImp(
            ApplicationConfigurationService appConfig,
            SportSplitterService sportSplitterService,
            IDynamicRepository repository)
        {
            this.appConfig = appConfig;
            this.sportSplitterService = sportSplitterService;
            this.repository = repository;
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
            if (appConfig.IsManual || !appConfig.IsCheckEnoughData)
            {
                return true;
            }

            return QueryDWStatus().DataIsReady;
        }

        private TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }
    }
}