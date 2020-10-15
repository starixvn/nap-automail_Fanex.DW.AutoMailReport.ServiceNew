using Fanex.Data.Repository;
using Fanex.Service.Core;
using Fanex.TopSportbookAgents.Business.Criterias;
using Fanex.TopSportbookAgents.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TopSportbookAgents.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryTopSportbookAgents(int winloss);

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

        public List<PunterModel> QueryTopSportbookAgents(int winloss)
        {
            var criteria = new Criteria
            {
                RptDate = dateRangeService.RptDate,
                Winloss = winloss
            };

            return repository.Fetch<PunterModel>(criteria).ToList();
        }

        public bool IsDataAvailable()
        {
            var result = false;
            if (AppConfigService.Config.IsManual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            var status = repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
            result = status.DataIsReady;

            return result;
        }
    }
}