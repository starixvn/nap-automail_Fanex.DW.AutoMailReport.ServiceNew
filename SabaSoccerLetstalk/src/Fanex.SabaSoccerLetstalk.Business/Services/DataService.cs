using Fanex.Data.Repository;
using Fanex.SabaSoccerLetstalk.Business.Criterias;
using Fanex.SabaSoccerLetstalk.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryDataSabaSoccer();

        List<PunterModel> QueryDataSabaHourly(DateTime date, LetstalkTodayPunterSetting setting);

        bool IsDataAvailable();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateRangeService dateRangeService;

        public DataServiceImp(DateRangeService dateRangeService, IDynamicRepository repository)
        {
            this.dateRangeService = dateRangeService;
            this.repository = repository;
        }

        public List<PunterModel> QueryDataSabaSoccer()
        {
            var criteria = new PunterLeagueGroupSportCriteria { RptDate = dateRangeService.RptDate };
            return repository.Fetch<PunterModel>(criteria).ToList();
        }

        public bool IsDataAvailable()
        {
            if (AppConfigService.Config.IsManual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            return QueryDWStatus().DataIsReady;
        }

        private TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        public List<PunterModel> QueryDataSabaHourly(DateTime date, LetstalkTodayPunterSetting setting)
        {
            var criteria = new PunterLeagueGroupSportHourlyCriteria { RptDate = date, SPName = setting.SPName };
            return repository.Fetch<PunterModel>(criteria).ToList();
        }
    }
}