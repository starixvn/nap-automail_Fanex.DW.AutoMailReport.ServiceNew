using System.Collections.Generic;
using Fanex.Data.Repository;
using TOControl.Notification.Business.Models;
using TOControl.Notification.Business.Repositories;

namespace TOControl.Notification.Business.Services
{
    public interface TOControlRepositoryService
    {
        CustomerInfomation QueryCustomerByCustId(int custId);

        IEnumerable<T> QueryRawData<T>(CriteriaBase criteria);

        T QuerySingleRow<T>(CriteriaBase criteria);
    }

    public class TOControlRepositoryServiceImp : TOControlRepositoryService
    {
        private readonly IDynamicRepository dynamicRepository;

        public TOControlRepositoryServiceImp(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public CustomerInfomation QueryCustomerByCustId(int custId)
        {
            var criteria = new CustomerSelByUserNameCriteria { CustId = custId };
            return dynamicRepository.Get<CustomerInfomation>(criteria);
        }

        public IEnumerable<T> QueryRawData<T>(CriteriaBase criteria)
        {
            return dynamicRepository.Fetch<T>(criteria);
        }

        public T QuerySingleRow<T>(CriteriaBase criteria)
        {
            return dynamicRepository.Get<T>(criteria);
        }
    }
}