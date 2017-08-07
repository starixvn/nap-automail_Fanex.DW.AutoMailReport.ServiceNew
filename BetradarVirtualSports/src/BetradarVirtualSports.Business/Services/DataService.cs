using System;
using System.Collections.Generic;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Repositories;
using Fanex.Data.Repository;
using Fanex.Service.Core;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public interface DataService
    {
        Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>> QueryDaily(DateTime date);
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

        public Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>> QueryDaily(DateTime date)
        {
            return dynamicRepository.FetchMultiple<GrossWinningDaily, GrossWinningDaily>(new DailyCriteria { Date = date });
        }

        public DateTime QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new DWStatusByCubeCriteria()).LastDate;
        }
    }
}