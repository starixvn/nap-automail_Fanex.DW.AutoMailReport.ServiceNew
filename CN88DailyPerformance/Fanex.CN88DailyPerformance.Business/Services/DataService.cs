using Fanex.CN88DailyPerformance.Service.Criteria;
using Fanex.CN88DailyPerformance.Service.Model;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;

namespace Fanex.CN88DailyPerformance.Service.Services
{
    public interface DataService
    {
        IEnumerable<DailyPerformanceModel> Query(DateTime reportDate);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;

        public DataServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<DailyPerformanceModel> Query(DateTime reportDate)
        {
            var criteria = new DailyPerformanceCriteria
            {
                FromDate = reportDate,
                ToDate = reportDate
            };

            return repository.Fetch<DailyPerformanceModel>(criteria);
        }
    }
}
