using Fanex.Data.Repository;
using Fanex.SabaSoccerLetstalk.Business.Criterias;
using Fanex.SabaSoccerLetstalk.Business.Models;
using Fanex.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryDataSabaSoccer();

        List<PunterModel> QueryDataSabaSoccerHourly(DateTime date, string spName, string leagueGroup);

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
            var result = false;
            if (AppConfigService.Config.IsManual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            var lastDate = repository.Get<LastDateModel>(new GetDWStatusByCubeCriteria());
            result = lastDate.LastDate >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }

        public List<PunterModel> QueryDataSabaSoccerHourly(DateTime date, string spName, string leagueGroup)
        {
            var criteria = new PunterLeagueGroupSportHourlyCriteria { RptDate = date, SPName = spName, LeagueGroup = leagueGroup };
            return repository.Fetch<PunterModel>(criteria).ToList();
        }
    }
}