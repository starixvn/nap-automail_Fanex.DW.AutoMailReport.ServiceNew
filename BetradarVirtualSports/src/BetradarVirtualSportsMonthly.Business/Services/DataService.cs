using System;
using System.Collections.Generic;
using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Repositories;
using Fanex.Data.Repository;
using Fanex.Service.Core;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public interface DataService
    {
        Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>> QueryMonthly(DateTime fromDate, DateTime toDate);
    }

    public class RepositoryServiceImp : RepositoryService, DataService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ApplicationConfigurationService appConfigService;

        public RepositoryServiceImp(IDynamicRepository dynamicRepository, ApplicationConfigurationService appConfigService)
        {
            this.dynamicRepository = dynamicRepository;
            this.appConfigService = appConfigService;
        }

        public bool IsDataAvailable()
        {
            if (appConfigService.Configurations.IsManual || !appConfigService.Configurations.IsCheckEnoughData)
            {
                return true;
            }

            var result = QueryLastDate() >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }

        public Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>> QueryMonthly(DateTime fromDate, DateTime toDate)
        {
            return dynamicRepository.FetchMultiple<GrossWinningMonthly, GrossWinningMonthly>(new MonthlyCriteria { FromDate = fromDate, ToDate = toDate });
        }

        public DateTime QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new DWStatusByCubeCriteria()).LastDate;
        }
    }
}