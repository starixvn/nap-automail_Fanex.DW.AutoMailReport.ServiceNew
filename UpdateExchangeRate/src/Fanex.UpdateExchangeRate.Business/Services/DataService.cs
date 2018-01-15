using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Fanex.UpdateExchangeRate.Business.Models;
using Fanex.UpdateExchangeRate.Business.Repositories;

namespace Fanex.UpdateExchangeRate.Business.Services
{
    public interface DataService
    {
        List<CurrencyRate> Query();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository dynamicRepository;

        public DataServiceImp(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public List<CurrencyRate> Query()
        {
            return dynamicRepository.Fetch<CurrencyRate>(new Criteria()).ToList();
        }
    }
}