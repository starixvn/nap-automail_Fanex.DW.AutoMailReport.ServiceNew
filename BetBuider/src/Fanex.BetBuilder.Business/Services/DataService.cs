using System.Collections.Generic;
using System.Linq;
using Fanex.BetBuilder.Business.Criterias;
using Fanex.BetBuilder.Business.Models;
using Fanex.Data.Repository;

namespace Fanex.BetBuilder.Business.Services
{
    public interface DataService
    {
        List<DailyRawDataModel> QueryDailyData();

        List<MonthlyRawDataModel> QueryMonthlyData();
     }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly CriteriaService param;

        public DataServiceImp(IDynamicRepository repository, CriteriaService param)
        {
            this.repository = repository;
            this.param = param;
        }

        public List<DailyRawDataModel> QueryDailyData()
        {
            var criteria = new DailyBetBuilderCriteria
            {
                WinlostDate = param.WinlostDate
            };

            return repository.Fetch<DailyRawDataModel>(criteria).ToList();
        }

        public List<MonthlyRawDataModel> QueryMonthlyData()
        {
            var criteria = new MonthlyBetBuilderCriteria
            {
                RptDate = param.WinlostDate
            };

            return repository.Fetch<MonthlyRawDataModel>(criteria).ToList();
        }
    }
}