using System;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Repositories;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ApplicationConfigurationService appConfigService;

        public RepositoryServiceImp(IDynamicRepository dynamicRepository, ApplicationConfigurationService appConfigService)
        {
            this.dynamicRepository = dynamicRepository;
            this.appConfigService = appConfigService;
        }

        public DateTime QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new DWStatusByCubeCriteria()).LastDate;
        }

        public bool IsDataAvailable()
        {
            if (appConfigService.Configurations.ReportDateRange == ReportDateRange.Manual || !appConfigService.Configurations.IsCheckEnoughData)
            {
                return true;
            }

            var result = QueryLastDate() >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }
    }
}