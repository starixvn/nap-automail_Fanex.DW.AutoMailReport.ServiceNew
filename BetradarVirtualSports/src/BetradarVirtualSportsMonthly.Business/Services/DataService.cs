using BetradarVirtualSportsMonthly.Business.Models;
using BetradarVirtualSportsMonthly.Business.Repositories;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public interface DataService
    {
        Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>> QueryMonthly(DateTime fromDate, DateTime toDate);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository dynamicRepository;

        public DataServiceImp(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Tuple<IEnumerable<GrossWinningMonthly>, IEnumerable<GrossWinningMonthly>> QueryMonthly(DateTime fromDate, DateTime toDate)
        {
            return dynamicRepository.FetchMultiple<GrossWinningMonthly, GrossWinningMonthly>(new MonthlyCriteria { FromDate = fromDate, ToDate = toDate });
        }
    }
}