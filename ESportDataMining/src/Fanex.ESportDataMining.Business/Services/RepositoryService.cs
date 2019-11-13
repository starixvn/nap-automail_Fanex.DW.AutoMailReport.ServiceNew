using Fanex.Data.Repository;
using Fanex.ESportDataMining.Business.Criterias;
using Fanex.ESportDataMining.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.ESportDataMining.Business.Services
{
    public interface RepositoryService
    {
        List<BettypeModel> QueryBettype();

        List<FactCustEsportModel> QueryCustESport();

        List<LiveStatusModel> QueryLiveStatus();

        List<CurrencyModel> QueryCurrency();

        List<SiteModel> QuerySite();

        List<MatchModel> QueryMatch();

        List<DateModel> QueryDate();

        List<LeagueModel> QueryLeague();

        List<CustomerModel> QueryCustomer();

        List<TraderModel> QueryTrader();
    }

    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeService dateTimeService;

        public RepositoryServiceImp(IDynamicRepository repository, DateTimeService dateTimeService)
        {
            this.repository = repository;
            this.dateTimeService = dateTimeService;
        }

        public List<BettypeModel> QueryBettype()
        {
            var criteria = new BettypeCriteria();
            return repository.Fetch<BettypeModel>(criteria).ToList();
        }

        public List<FactCustEsportModel> QueryCustESport()
        {
            var criteria = new FactCustEsportCriteria { FromDate = dateTimeService.FromDate(), ToDate = dateTimeService.ToDate() };
            return repository.Fetch<FactCustEsportModel>(criteria).ToList();
        }

        public List<LiveStatusModel> QueryLiveStatus()
        {
            var criteria = new LiveStatusCriteria();
            return repository.Fetch<LiveStatusModel>(criteria).ToList();
        }

        public List<CurrencyModel> QueryCurrency()
        {
            var criteria = new CurrencyCriteria();
            return repository.Fetch<CurrencyModel>(criteria).ToList();
        }

        public List<SiteModel> QuerySite()
        {
            var criteria = new SiteCriteria();
            return repository.Fetch<SiteModel>(criteria).ToList();
        }

        public List<MatchModel> QueryMatch()
        {
            var criteria = new MatchCriteria { FromDate = dateTimeService.FromDate(), ToDate = dateTimeService.ToDate() };
            return repository.Fetch<MatchModel>(criteria).ToList();
        }

        public List<DateModel> QueryDate()
        {
            var criteria = new DateCriteria();
            return repository.Fetch<DateModel>(criteria).ToList();
        }

        public List<LeagueModel> QueryLeague()
        {
            var criteria = new LeagueCritetia();
            return repository.Fetch<LeagueModel>(criteria).ToList();
        }

        public List<CustomerModel> QueryCustomer()
        {
            var criteria = new CustomerCriteria { FromDate = dateTimeService.FromDate(), ToDate = dateTimeService.ToDate() };
            return repository.Fetch<CustomerModel>(criteria).ToList();
        }

        public List<TraderModel> QueryTrader()
        {
            var critera = new TraderCriteria { FromDate = dateTimeService.FromDate(), ToDate = dateTimeService.ToDate() };
            return repository.Fetch<TraderModel>(critera).ToList();
        }
    }
}