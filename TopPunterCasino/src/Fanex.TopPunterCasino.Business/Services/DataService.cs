using Fanex.Data.Repository;
using Fanex.Service.Core;
using Fanex.TopPunterCasino.Business.Criterias;
using Fanex.TopPunterCasino.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TopPunterCasino.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryCasinoTopPunterData(int totalTurnover, string productIds);

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

        public List<PunterModel> QueryCasinoTopPunterData(int totalTurnover, string productIds)
        {
            var casinoCriteria = new CasinoTopPunterCriteria
            {
                RptDate = dateRangeService.RptDate,
                TotalTurnover = totalTurnover
            };

            var nonSportCriteria = new PunterNonSportCriteria
            {
                RptDate = dateRangeService.RptDate,
                TotalTurnover = totalTurnover,
                ProductIds = productIds
            };

            var casinoData = repository.Fetch<PunterModel>(casinoCriteria).ToList();
            var nonSportData = repository.Fetch<PunterModel>(nonSportCriteria).ToList();

            return casinoData.Concat(nonSportData).ToList();
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
    }
}