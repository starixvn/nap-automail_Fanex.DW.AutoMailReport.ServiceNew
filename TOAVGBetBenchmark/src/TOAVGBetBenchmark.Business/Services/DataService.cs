using Fanex.Data.Repository;
using Fanex.TOAVGBetBenchmark.Business.Criterias;
using Fanex.TOAVGBetBenchmark.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TOAVGBetBenchmark.Business.Services
{
    public interface DataService
    {
        List<BenchmarkModel> QueryTOAVGBetBenchmark();
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

        public List<BenchmarkModel> QueryTOAVGBetBenchmark()
        {
            var criteria = new Criteria
            {
                RptDate = dateRangeService.RptDate
            };

            return repository.Fetch<BenchmarkModel>(criteria).ToList();
        }
    }
}