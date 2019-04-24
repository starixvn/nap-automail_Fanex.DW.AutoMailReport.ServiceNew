using System;
using System.Collections.Generic;
using System.Linq;
using NegativeBalance.Business.Models;

namespace NegativeBalance.Business.Services
{
    public interface DataService
    {
        Tuple<List<Match>, string> QueryMatchesBySite(Site site);
    }

    public class DataServiceImp : DataService
    {
        private const double EPSILON = 0.00000001;
        private readonly RepositoryBalanceService repository;
        private readonly ApplicationConfigurationService appConfigService;

        public DataServiceImp(
            RepositoryBalanceService repository,
            ApplicationConfigurationService appConfigService)
        {
            this.repository = repository;
            this.appConfigService = appConfigService;
        }

        public Tuple<List<Match>, string> QueryMatchesBySite(Site site)
        {
            var matches = new List<Match>();
            var customers = repository.QueryCustomerBalanceInfoBySite(site.LicenseeName, appConfigService.Configurations.IsTest);
            var matchIdsValid = new List<int>();

            UpdateGroupId(customers);
            customers.GroupBy(g => new { g.MatchId, g.GroupId })
                .Distinct()
                .ToList()
                .ForEach(match =>
                {
                    var matchSettledLog = repository.QueryMatchSettledLog(site.LicenseeName, match.Key.MatchId, match.Key.GroupId);
                    if (matchSettledLog.Item1.IsValid)
                    {
                        matchIdsValid.Add(match.Key.MatchId);
                        var rawCustomersByMatch = customers.Where(s => s.MatchId == match.Key.MatchId && s.GroupId == match.Key.GroupId).ToList();
                        var custombersByMatch = DistinctCustomers(rawCustomersByMatch);
                        matches.Add(new Match
                        {
                            LicenseeName = site.LicenseeName,
                            MatchInfo = matchSettledLog.Item1,
                            MatchSettleLogList = matchSettledLog.Item2,
                            CustomerInfoList = custombersByMatch
                        });
                    }
                });

            var logIds = GetLogIds(customers, matchIdsValid);

            return new Tuple<List<Match>, string>(matches, logIds);
        }

        private static void UpdateGroupId(List<CustomerBalanceInfo> customers)
        {
            const int GroupId = 2;
            customers.ForEach(custInfo =>
            {
                custInfo.GroupId = custInfo.GroupId == GroupId ? 1 : custInfo.GroupId;
            });
        }

        private static string GetLogIds(List<CustomerBalanceInfo> customers, List<int> matchIdsValid)
        {
            var logIds = customers.Where(s => matchIdsValid.Contains(s.MatchId)).Select(c => c.LogId).ToList();
            return string.Join(",", logIds);
        }

        private static List<CustomerBalanceInfo> DistinctCustomers(List<CustomerBalanceInfo> rawCustomers)
        {
            var result = new List<CustomerBalanceInfo>();
            rawCustomers
                .GroupBy(c => new { c.MatchId, c.CustId, c.TotalBalance })
                .Distinct()
                .ToList()
                .ForEach(id =>
                {
                    var customer = rawCustomers.FirstOrDefault(c => c.CustId == id.Key.CustId
                    && c.MatchId == id.Key.MatchId
                    && Math.Abs(c.TotalBalance - id.Key.TotalBalance) < EPSILON);
                    result.Add(customer);
                });

            return result;
        }
    }
}