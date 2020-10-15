using Fanex.Data.Repository;
using Fanex.N88DailyWL.Business.Criterias;
using Fanex.N88DailyWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface DataService
    {
        Tuple<List<N88CurrencyModel>, List<N88ProductModel>> QueryN88DataYesterday();

        Tuple<List<N88CurrencyModel>, List<N88ProductModel>> QueryN88DataMTD();
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

        public Tuple<List<N88CurrencyModel>, List<N88ProductModel>> QueryN88DataYesterday()
        {
            var criteria = new N88Criteria { FromDate = dateService.ToDate, ToDate = dateService.ToDate };
            var data = repository.FetchMultiple<N88CurrencyModel, N88ProductModel>(criteria);
            var criteriaSB = new N88SportbookCriteria { FromDate = dateService.ToDate, ToDate = dateService.ToDate };
            var dataSB = repository.Fetch<N88ProductModel>(criteriaSB).ToList();
            dataSB.AddRange(data.Item2.ToList());

            return new Tuple<List<N88CurrencyModel>, List<N88ProductModel>>
            (
                data.Item1.ToList(),
                dataSB
            );
        }

        public Tuple<List<N88CurrencyModel>, List<N88ProductModel>> QueryN88DataMTD()
        {
            var criteria = new N88Criteria { FromDate = dateService.FromDate, ToDate = dateService.ToDate };
            var data = repository.FetchMultiple<N88CurrencyModel, N88ProductModel>(criteria);
            var criteriaSB = new N88SportbookCriteria { FromDate = dateService.FromDate, ToDate = dateService.ToDate };
            var dataSB = repository.Fetch<N88ProductModel>(criteriaSB).ToList();
            dataSB.AddRange(data.Item2.ToList());

            return new Tuple<List<N88CurrencyModel>, List<N88ProductModel>>
            (
                data.Item1.ToList(),
                dataSB
            );
        }
    }
}