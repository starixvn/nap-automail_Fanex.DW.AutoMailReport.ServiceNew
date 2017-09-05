using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using TOControl.CustomerMovement.Business.Models;
using TOControl.CustomerMovement.Business.Repositories;

namespace TOControl.CustomerMovement.Business.Services
{
    public interface DataService
    {
        Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> Query();
    }

    public class DataServiceImp : DataService
    {
        private readonly DateService dateService;
        private readonly IDynamicRepository dynamicRepository;

        public DataServiceImp(DateService dateService, IDynamicRepository dynamicRepository)
        {
            this.dateService = dateService;
            this.dynamicRepository = dynamicRepository;
        }

        public Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> Query()
        {
            var criteria = new Criteria
            {
                FromDate = dateService.FromDate,
                ToDate = dateService.ToDate
            };

            var groupHeadline = dynamicRepository.Fetch<GroupHeadline>(criteria);
            var customerMovements = dynamicRepository.Fetch<Movement>(criteria);
            var newDirectCustomers = dynamicRepository.Fetch<DirectCustomer>(criteria);

            UpdateNumberOfGroupHeadline(customerMovements);

            return new Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>>(groupHeadline, customerMovements, newDirectCustomers);
        }

        private static void UpdateNumberOfGroupHeadline(IEnumerable<Movement> customerMovements)
        {
            foreach (var m in customerMovements)
            {
                m.Count = customerMovements.Count(c => c.GroupHeadline == m.GroupHeadline);
            }
        }
    }
}