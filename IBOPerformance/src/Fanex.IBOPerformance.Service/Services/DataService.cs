using Fanex.Data.Repository;
using Fanex.IBOPerformance.Service.Criterias;
using Fanex.IBOPerformance.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Fanex.Service.Core;

namespace Fanex.IBOPerformance.Service.Services
{
    public interface DataService
    {
        List<DailyCreatedCustomerModel> QueryDailyCreatedCustomer(string site, int currencyId);

        List<DailyPerformanceModel> QueryDailyPerformance(string site, int currencyId);

        bool IsDataAvailable();

        LastDateModel QueryLastDate();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateService dateService;

        public DataServiceImp(IDynamicRepository repository, DateService dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public bool IsDataAvailable()
        {
            if (AppConfigService.AppConfig.IsManual || !AppConfigService.AppConfig.IsCheckEnoughData)
            {
                return true;
            }

            return QueryLastDate().LastDate >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());
        }

        public List<DailyCreatedCustomerModel> QueryDailyCreatedCustomer(string site, int currencyId)
        {
            var yesterday = dateService.GetYesterday();
            var criteria = new DailyCreatedCriteria { Site = site, CurrencyId = currencyId, Yesterday = yesterday };
            return repository.Fetch<DailyCreatedCustomerModel>(criteria).ToList();
        }

        public List<DailyPerformanceModel> QueryDailyPerformance(string site, int currencyId)
        {
            var yesterday = dateService.GetYesterday();
            var criteria = new DailyPerformanceCriteria { Site = site, CurrencyId = currencyId, Yesterday = yesterday };
            return repository.Fetch<DailyPerformanceModel>(criteria).ToList();
        }      

        public LastDateModel QueryLastDate()
        {
            return repository.Get<LastDateModel>(new GetDWStatusByCubeCriteria());
        }
    }
}