using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Repositories;

namespace NegativeBalance.Business.Services
{
    public interface RepositoryBalanceService
    {
        List<Site> QuerySites(bool isTest);

        List<CustomerBalanceInfo> QueryCustomerBalanceInfoBySite(string site, bool isTest);

        Tuple<MatchInfo, List<MatchSettleLog>> QueryMatchSettledLog(string licenseeName, int matchId, int groupId);

        bool UpdateMatchSent(string logIds);
    }

    public class RepositoryBalanceServiceImp : RepositoryBalanceService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly TransformService transformService;

        public RepositoryBalanceServiceImp(IDynamicRepository dynamicRepository, TransformService transformService)
        {
            this.dynamicRepository = dynamicRepository;
            this.transformService = transformService;
        }

        public List<Site> QuerySites(bool isTest)
        {
            if (isTest)
            {
                return new List<Site>
                {
                    new Site { IsAvailable = true, LicenseeName = "spondemo", ToList = "testing@starixvn.com" },
                    new Site { IsAvailable = true, LicenseeName = "Athena000", ToList = "testing@starixvn.com" }
                };
            }

            return dynamicRepository.Fetch<Site>(new SelectMailingListCriteria())
                .Where(s => s.IsAvailable)
                .ToList();
        }

        public List<CustomerBalanceInfo> QueryCustomerBalanceInfoBySite(string site, bool isTest)
        {
            return dynamicRepository.Fetch<CustomerBalanceInfo>(new SelectBySiteCriteria { Site = site, IsTest = isTest }).ToList();
        }

        public Tuple<MatchInfo, List<MatchSettleLog>> QueryMatchSettledLog(string licenseeName, int matchId, int groupId)
        {
            var result = dynamicRepository.FetchMultiple<RawMatchInfo, MatchSettleLog>(new SelectMatchLogCriteria { MatchId = matchId, GroupId = groupId });
            var matchInfo = transformService.Execute(licenseeName, result.Item1.FirstOrDefault(), groupId);

            return new Tuple<MatchInfo, List<MatchSettleLog>>(matchInfo, result.Item2.ToList());
        }

        public bool UpdateMatchSent(string logIds)
        {
            var command = new UpdateMatchSentCommand { LogIds = logIds };
            dynamicRepository.Execute(command);

            return command.ErrorCode == 0;
        }
    }
}