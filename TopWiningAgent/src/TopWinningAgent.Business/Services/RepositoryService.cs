using System;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using TopWinningAgent.Business.Repositories;

namespace TopWinningAgent.Business.Services
{
    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ApplicationConfigurationService appConfig;

        public RepositoryServiceImp(IDynamicRepository dynamicRepository, ApplicationConfigurationService appConfig)
        {
            this.dynamicRepository = dynamicRepository;
            this.appConfig = appConfig;
        }

        public bool IsDataAvailable()
        {
            if (!appConfig.Configurations.IsCheckEnoughData)
            {
                return true;
            }

            var result = QueryLastDate() >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }

        public DateTime QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new DWStatusByCubeCriteria()).LastDate;
        }
    }
}