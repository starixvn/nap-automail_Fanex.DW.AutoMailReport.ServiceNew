using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Data;
using TopWinningAgent.Business.Models;
using TopWinningAgent.Business.Repositories;

namespace TopWinningAgent.Business.Services
{
    public interface DataService
    {
        DataTable QueryData(DateTime? dateServiceRun);

        bool IsDataAvailable();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ApplicationConfigurationService appConfig;

        public DataServiceImp(IDynamicRepository dynamicRepository, ApplicationConfigurationService appConfig)
        {
            this.dynamicRepository = dynamicRepository;
            this.appConfig = appConfig;
        }

        public bool IsDataAvailable()
        {
            if (!appConfig.Configurations.IsCheckEnoughData || appConfig.Configurations.IsManual)
            {
                return true;
            }

            return QueryDWStatus().DataIsReady;
        }

        private TaskstatusModel QueryDWStatus()
        {
            return dynamicRepository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        public DataTable QueryData(DateTime? dateServiceRun)
        {
            var result = new DataTable();
            var criteria = new Criteria { DateServiceRun = dateServiceRun };
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey(), 0))
            {
                if (dateServiceRun.HasValue)
                {
                    db.SetParameterValue("dateservicerun", dateServiceRun);
                }

                var ds = db.ExecuteDataSet();
                if (ds.Tables.Count > 0)
                {
                    result = ds.Tables[0];
                }
            }

            return result;
        }
    }
}