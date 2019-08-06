using Fanex.Data.Repository;
using Fanex.FinanceTransfer.Business.Criterias;
using Fanex.FinanceTransfer.Business.Models;
using System;

namespace Fanex.FinanceTransfer.Business.Services
{
    public interface RepositoryService
    {
        Result Execute(DateTime moveBetsDay, DateTime? transferMonthDate);

        MoveBetDay SelMoveBetDay();
    }

    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository repository;

        public RepositoryServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public Result Execute(DateTime moveBetsDay, DateTime? transferMonthDate)
        {
            var criteria = new TransferCriteria
            {
                MoveBetsDay = moveBetsDay,
                TransferMonthDate = transferMonthDate
            };

            repository.Execute(criteria);

            return new Result
            {
                ErrCode = criteria.ErrCode,
                ErrMsg = criteria.ErrMsg
            };
        }

        public MoveBetDay SelMoveBetDay()
        {
            return repository.Get<MoveBetDay>(new SelBalanceUpDayCriteria());
        }
    }
}