using Fanex.AbnormalBets.Business.Criterias;
using Fanex.AbnormalBets.Business.Models.CSD;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface RepositoryCSDService
    {
        List<AbnormalBetDailyModel> QueryBetTickets(DateTime rptDate);
    }

    public class RepositoryCSDServiceImp : RepositoryCSDService
    {
        private readonly IDynamicRepository repository;

        public RepositoryCSDServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public List<AbnormalBetDailyModel> QueryBetTickets(DateTime rptDate)
        {
            var criteria = new AbnormalCSDCriteria { RptDate = rptDate, IsTest = AppConfigService.AppConfig.IsTest };
            return repository.Fetch<AbnormalBetDailyModel>(criteria).ToList();
        }
    }
}