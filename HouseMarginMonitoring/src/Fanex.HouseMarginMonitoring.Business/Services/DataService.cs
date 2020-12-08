using Fanex.Data.Repository;
using Fanex.HouseMarginMonitoring.Business.Criterias;
using Fanex.HouseMarginMonitoring.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.HouseMarginMonitoring.Business.Services
{
    public interface DataService
    {
        List<BettypeModel> QueryData();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;

        public DataServiceImp(IDynamicRepository repository, DateTimeServiceProvider dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public List<BettypeModel> QueryData()
        {
            var criteria = new Criteria
            {
                FromDate = dateService.DateTime.FromDate,
                ToDate = dateService.DateTime.ToDate,
                WLThreshold = AppConfigService.Config.WLThreshold,
                MarginThreshold = AppConfigService.Config.MarginThreshold
            };

            var data = repository.Fetch<BettypeModel>(criteria).ToList();
            int order = 1;
            data.ForEach(row =>
            {
                row.Order = order;
                order++;
            });

            data.Add(new BettypeModel
            {
                BettypeName = "Total",
                HouseTO = data.Sum(s => s.HouseTO),
                HouseWL = data.Sum(s => s.HouseWL)
            });

            return data;
        }
    }
}