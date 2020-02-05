using Fanex.Data.Repository;
using Fanex.N88DailyWL.Business.Criterias;
using Fanex.N88DailyWL.Business.Models;
using System;
using System.Linq;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface DataService
    {
        Tuple<N88DailyWLModel, N88DailyWLModel, DepositModel> QueryData();
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

        public Tuple<N88DailyWLModel, N88DailyWLModel, DepositModel> QueryData()
        {
            var criteria = new N88Criteria { FromDate = dateService.FromDate, ToDate = dateService.ToDate };
            var data = repository.FetchMultiple<N88DailyWLModel, N88DailyWLModel, DepositModel>(criteria);
            var yesterday = data.Item1.FirstOrDefault();
            var mtd = data.Item2.FirstOrDefault();
            var deposit = data.Item3.FirstOrDefault();

            return new Tuple<N88DailyWLModel, N88DailyWLModel, DepositModel>
                (
                    yesterday,
                    mtd,
                    deposit
                );
        }
    }
}