using EventMarketOffer.Business.Repositories;
using Fanex.Data;
using System.Data;

namespace EventMarketOffer.Business.Services
{
    public interface DataService
    {
        DataTable Query();
    }

    public class DataServiceImp : DataService
    {
        private readonly DateService dateService;
        private readonly AppConfigurationService appConfig;

        public DataServiceImp(DateService dateService,
                              AppConfigurationService appConfig)
        {
            this.dateService = dateService;
            this.appConfig = appConfig;
        }

        public DataTable Query()
        {
            var criteria = new CriteriaEventMartketOffer { DateStart = dateService.DateStart, DateEnd = dateService.DateEnd };
            DataTable dataTable = new DataTable();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey(), 0))
            {
                db.SetParameterValue("datestart", criteria.DateStart);
                db.SetParameterValue("dateend", criteria.DateEnd);
                var dataset = db.ExecuteDataSet();
                if (dataset.Tables.Count > 0)
                {
                    dataTable = dataset.Tables[0];
                }
            }

            return dataTable;
        }

        public bool IsDataAvailable()
        {
            if (appConfig.Configurations.IsManual || !appConfig.Configurations.IsCheckEnoughData)
            {
                return true;
            }

            return true;
        }
    }
}