using Fanex.Data.Repository;
using Fanex.LicenseeFinanceCode.Business.Criteria;
using Fanex.LicenseeFinanceCode.Business.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.LicenseeFinanceCode.Business.Service
{
    public interface DataService
    {
        List<LicenseeTransferCodeModel> Query();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;

        public DataServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public List<LicenseeTransferCodeModel> Query()
        {
            var balanceUpday = SelBalanceUpDay();
            var criteria = new LicenseeTransferCodeSelectCriteria
            {
                MoveBetsDay = balanceUpday.MoveBetsDay
            };

            return repository.Fetch<LicenseeTransferCodeModel>(criteria).ToList();
        }

        private SelBalanceUpDayModel SelBalanceUpDay()
        {
            var criteria = new SelBalanceUpDayCriteria();
            return repository.Get<SelBalanceUpDayModel>(criteria);
        }
    }
}