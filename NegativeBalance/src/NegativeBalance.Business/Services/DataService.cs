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
            var logIds = GetLogIds(customers);
            customers.GroupBy(g => new { g.MatchId, g.GroupId })
                .Distinct()
                .ToList()
                .ForEach(match =>
                {
                    var matchSettledLog = repository.QueryMatchSettledLog(match.Key.MatchId, match.Key.GroupId);
                    var rawCustomersByMatch = customers.Where(s => s.MatchId == match.Key.MatchId && s.GroupId == match.Key.GroupId).ToList();
                    var custombersByMatch = DistinctCustomers(rawCustomersByMatch);
                    matches.Add(new Match
                    {
                        LicenseeName = site.LicenseeName,
                        MatchInfo = matchSettledLog.Item1,
                        MatchSettleLogList = matchSettledLog.Item2,
                        CustomerInfoList = custombersByMatch
                    });
                });

            return new Tuple<List<Match>, string>(matches, logIds);
        }

        private static string GetLogIds(List<CustomerBalanceInfo> customers)
        {
            var logIds = customers.Select(c => c.LogId).ToList();
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