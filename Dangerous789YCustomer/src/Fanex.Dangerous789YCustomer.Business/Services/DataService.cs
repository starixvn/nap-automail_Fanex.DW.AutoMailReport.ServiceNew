using Fanex.Dangerous789YCustomer.Business.Criterias;
using Fanex.Dangerous789YCustomer.Business.Models;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Dangerous789YCustomer.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryDangerous789YCustomer(int threshold);
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

        public List<PunterModel> QueryDangerous789YCustomer(int threshold)
        {
            var criteria = new Criteria
            {
                CheckDayGMTP8 = dateRangeService.RptDate,
                Threshold = threshold
            };

            return repository.Fetch<PunterModel>(criteria).ToList();
        }
    }
}