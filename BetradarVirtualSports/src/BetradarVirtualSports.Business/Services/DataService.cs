using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Repositories;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using System;
using System.Collections.Generic;

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

            var result = QueryStatus();

            return result;
        }

        public Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>> QueryDaily(DateTime date)
        {
            return dynamicRepository.FetchMultiple<GrossWinningDaily, GrossWinningDaily>(new DailyCriteria { Date = date });
        }

        public bool QueryStatus()
        {
            return dynamicRepository.Get<TaskstatusModel>(new DWStatusByCubeCriteria()).DataIsReady;
        }

        public DateTime QueryLastDate()
        {
            return DateTime.Today;
        }
    }
}