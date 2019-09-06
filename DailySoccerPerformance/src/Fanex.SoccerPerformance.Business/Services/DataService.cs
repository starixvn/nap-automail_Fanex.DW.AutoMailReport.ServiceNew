using Fanex.Data.Repository;
using Fanex.Service.Core;
using Fanex.SoccerPerformance.Business.Criterias;
using Fanex.SoccerPerformance.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface DataService
    {
        Tuple<IEnumerable<SoccerModel>, IEnumerable<SoccerModel>> Query(DateTime startDate, DateTime endDate);

        bool IsDataAvailable();

        LastDateModel QueryLastDate();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository dynamicRepository;

        public DataServiceImp(DateServiceProvider dateService, IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Tuple<IEnumerable<SoccerModel>, IEnumerable<SoccerModel>> Query(DateTime startDate, DateTime endDate)
        {
            var criteria = new SoccerCriteria
            {
                StartDate = startDate,
                EndDate = endDate
            };

            return dynamicRepository.FetchMultiple<SoccerModel, SoccerModel>(criteria);
        }

        public LastDateModel QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new GetDWStatusByCubeCriteria());
        }

        public bool IsDataAvailable()
        {
            var result = false;
            if (AppConfigService.AppConfig.IsManual|| !AppConfigService.AppConfig.IsCheckEnoughData)
            {
                return true;
            }

            result = QueryLastDate().LastDate >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }
    }
}
