using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Data;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface RepositoryBettypeProductService
    {
        Dictionary<string, DataTable> QueryGeneralData(Dictionary<string, DataTable> rawDataSource);

        Dictionary<string, DataTable> QueryDetailData(Dictionary<string, DataTable> rawDataSource);
    }

    public class RepositoryServiceImp : RepositoryBettypeProductService
    {
        private readonly ApplicationConfigurationService appConfig;
        private readonly IDynamicRepository dynamicRepository;

        public RepositoryServiceImp(ApplicationConfigurationService appConfig, IDynamicRepository dynamicRepository)
        {
            this.appConfig = appConfig;
            this.dynamicRepository = dynamicRepository;
        }

        public Dictionary<string, DataTable> QueryGeneralData(Dictionary<string, DataTable> rawDataSource)
        {
            var dataSource = new Dictionary<string, DataTable>();
            var index = 0;
            const int MaxIndex = 2;
            foreach (var item in rawDataSource)
            {
                if (index < MaxIndex)
                {
                    dataSource.Add(item.Key, item.Value);
                }

                index++;
            }

            return dataSource;
        }

        public Dictionary<string, DataTable> QueryDetailData(Dictionary<string, DataTable> rawDataSource)
        {
            var dataSource = new Dictionary<string, DataTable>();
            var index = 0;
            foreach (var item in rawDataSource)
            {
                if (index > 1)
                {
                    dataSource.Add(item.Key, item.Value);
                }

                index++;
            }

            return dataSource;
        }

        public bool IsDataAvailable()
        {
            if (appConfig.IsManual || !appConfig.IsCheckEnoughData)
            {
                return true;
            }

            return true;
        }
    }
}