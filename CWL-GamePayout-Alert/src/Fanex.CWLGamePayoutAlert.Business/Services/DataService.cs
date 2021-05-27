using Fanex.CWLGamePayoutAlert.Business.Criterias;
using Fanex.CWLGamePayoutAlert.Business.Models;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.CWLGamePayoutAlert.Business.Services
{
    public interface DataService
    {
        List<PayoutModel> QueryPayout();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;
        private readonly Log log;

        public DataServiceImp(IDynamicRepository repository, DateTimeServiceProvider dateService, Log log)
        {
            this.repository = repository;
            this.dateService = dateService;
            this.log = log;
        }

        public List<PayoutModel> QueryPayout()
        {
            var criteria = new Criteria
            {
                FromGMT8Time = dateService.DateTime.FromGMT8Time,
                TimeInterval = AppConfigService.Config.TimeInterval,
                MWinLV = AppConfigService.Config.MWinLV,
                TLV = AppConfigService.Config.TLV
            };

            var json = JsonConvert.SerializeObject(criteria, Formatting.Indented);
            log.Log($"Fanex.CWLGamePayoutAlert.Business.Services criteria: {json}");

            var data = repository.Fetch<PayoutModel>(criteria).ToList();
            return data;
        }
    }
}