using System;
using System.Data;
using EventMarketOffer.Business.Repositories;
using Fanex.Data.Repository;
using Fanex.Service.Core;

namespace EventMarketOffer.Business.Services
{
    public interface DataService
    {
        DataTable Query();
    }

    public class DataServiceImp : DataService, RepositoryService
    {
        private readonly DateService dateService;
        private readonly DataSourceService dataSourceService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly AppConfigurationService appConfig;

        public DataServiceImp(DateService dateService, DataSourceService dataSourceService, IDynamicRepository dynamicRepository, AppConfigurationService appConfig)
        {
            this.dateService = dateService;
            this.dataSourceService = dataSourceService;
            this.dynamicRepository = dynamicRepository;
            this.appConfig = appConfig;
        }

        public DataTable Query()
        {
            var criteria = new CriteriaEventMartketOffer { DateStart = dateService.DateStart, DateEnd = dateService.DateEnd };
            return dataSourceService.Query(criteria);
        }

        public DateTime QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new DWStatusByCubeCriteria()).LastDate;
        }

        public bool IsDataAvailable()
        {
            if (appConfig.Configurations.IsManual || !appConfig.Configurations.IsCheckEnoughData)
            {
                return true;
            }

            var result = QueryLastDate() >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }
    }
}