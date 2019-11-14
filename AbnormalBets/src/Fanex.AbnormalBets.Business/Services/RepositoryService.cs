using Fanex.AbnormalBets.Business.Criterias;
using Fanex.AbnormalBets.Business.Models;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface RepositoryService
    {
        Tuple<List<SummaryBetModel>, List<DetailBetModel>> Query(string site, DateTime rptDate);
    }

    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository repository;

        public RepositoryServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public Tuple<List<SummaryBetModel>, List<DetailBetModel>> Query(string site, DateTime rptDate)
        {
            var criteria = new AbnormalCriteria { Site = site, RptDate = rptDate };
            var result = repository.FetchMultiple<SummaryBetModel, DetailBetModel>(criteria);
            return new Tuple<List<SummaryBetModel>, List<DetailBetModel>>(result.Item1.ToList(), result.Item2.ToList());
        }
    }
}