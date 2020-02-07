using Fanex.AbnormalBets.Business.Criterias;
using Fanex.AbnormalBets.Business.Models.DailyCSD;
using Fanex.AbnormalBets.Business.Models.DailyLicensee;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface RepositoryService
    {
        Tuple<List<SummaryBetModel>, List<DetailBetModel>, List<UserLocationModel>> QueryDailyLicenseeBet(string site, DateTime rptDate);

        List<MailModel> QueryMail(DateTime rptDate);

        List<AbnormalBetDailyModel> QueryDailyCSDBet(DateTime rptDate);

        LocationModel QueryIP(string ip);
    }

    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository repository;

        public RepositoryServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public List<AbnormalBetDailyModel> QueryDailyCSDBet(DateTime rptDate)
        {
            var criteria = new AbnormalDailyCSDCriteria { RptDate = rptDate, IsTest = AppConfigService.AppConfig.IsTest };
            return repository.Fetch<AbnormalBetDailyModel>(criteria).ToList();
        }

        public Tuple<List<SummaryBetModel>, List<DetailBetModel>, List<UserLocationModel>> QueryDailyLicenseeBet(string site, DateTime rptDate)
        {
            var criteria = new AbnormalCriteria { Site = site, RptDate = rptDate, IsTest = AppConfigService.AppConfig.IsTest };
            var result = repository.FetchMultiple<SummaryBetModel, DetailBetModel, UserLocationModel>(criteria);

            return new Tuple<List<SummaryBetModel>, List<DetailBetModel>, List<UserLocationModel>>(
                result.Item1.ToList(), 
                result.Item2.ToList(),
                result.Item3.ToList());
        }

        public LocationModel QueryIP(string ip)
        {
            ip = string.IsNullOrEmpty(ip) ? string.Empty : ip.Trim();
            var criteria = new GetIPInfoCriteria { IP = ip };
            return repository.Get<LocationModel>(criteria);
        }

        public List<MailModel> QueryMail(DateTime rptDate)
        {
            var date = rptDate.AddDays(1);
            var criteria = new CheckMailStatusCriteria { FromDate = date, ToDate = date };
            return repository.Fetch<MailModel>(criteria).ToList();
        }
    }
}