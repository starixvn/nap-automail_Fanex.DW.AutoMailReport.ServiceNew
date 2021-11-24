using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Repositories;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public interface DataService
    {
        Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>> QueryDaily(DateTime date);

        bool IsDataAvailable();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ApplicationConfigurationService appConfigService;

        public DataServiceImp(IDynamicRepository dynamicRepository, ApplicationConfigurationService appConfigService)
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

            return dynamicRepository.Get<TaskstatusModel>(new DWStatusByCubeCriteria()).DataIsReady;
        }

        public Tuple<IEnumerable<GrossWinningDaily>, IEnumerable<GrossWinningDaily>> QueryDaily(DateTime date)
        {
            return dynamicRepository.FetchMultiple<GrossWinningDaily, GrossWinningDaily>(new DailyCriteria { Date = date });
        }
    }
}