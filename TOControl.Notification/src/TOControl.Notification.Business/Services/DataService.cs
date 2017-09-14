using System;
using System.Collections.Generic;
using System.Linq;
using TOControl.Notification.Business.Enums;
using TOControl.Notification.Business.Models;
using TOControl.Notification.Business.Repositories;

namespace TOControl.Notification.Business.Services
{
    public interface DataService
    {
        List<int> Ids { get; }

        Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> Query(NotifiedByFunction notifiedByFunction);
    }

    public class DataServiceImp : DataService
    {
        private readonly DateService dateService;
        private readonly XMLService xmlService;
        private readonly TransformDataService transformDataService;
        private readonly TOControlRepositoryService repositoryService;

        public DataServiceImp(
            DateService dateService,
            XMLService xmlService,
            TransformDataService transformDataService,
            TOControlRepositoryService repositoryService)
        {
            this.dateService = dateService;
            this.xmlService = xmlService;
            this.transformDataService = transformDataService;
            this.repositoryService = repositoryService;

            Ids = new List<int>();
        }

        public List<int> Ids
        {
            get;
        }

        public Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> Query(NotifiedByFunction notifiedByFunction)
        {
            var groupHeadlines = QueryGroupHeadline(notifiedByFunction);
            var customerMovements = QueryCustomerMovement(notifiedByFunction);
            var newDirectCustomers = QueryDirectCustomer(notifiedByFunction);

            var idgroupHeadlines = groupHeadlines.Select(s => s.Id).ToList();
            var idcustomerMovements = customerMovements.Select(s => s.Id).ToList();
            var idnewDirectCustomers = newDirectCustomers.Select(s => s.Id).ToList();

            Ids.AddRange(idgroupHeadlines);
            Ids.AddRange(idcustomerMovements);
            Ids.AddRange(idnewDirectCustomers);

            return new Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>>(groupHeadlines, customerMovements, newDirectCustomers);
        }

        private List<GroupHeadline> QueryGroupHeadline(NotifiedByFunction notifiedByFunction)
        {
            var result = new List<GroupHeadline>();
            var criteria = new NotificationInfoSelectByFunctionCriteria
            {
                FromDate = dateService.FromDate,
                ToDate = dateService.ToDate,
                ActionIDs = "1003",
                NotifiedByFunctionFlag = (int)notifiedByFunction,
            };

            var rawXMLData = repositoryService.QueryRawData<RawData>(criteria);
            rawXMLData.ToList().ForEach(rawData =>
            {
                var xml = rawData.NotificationInfo;
                var groupHeadlineRaw = xmlService.Parser<GroupHeadlineRaw>(xml);
                var groupHeadline = transformDataService.TransformGroupHeadline(groupHeadlineRaw, rawData);
                result.Add(groupHeadline);
            });

            return result;
        }

        private List<Movement> QueryCustomerMovement(NotifiedByFunction notifiedByFunction)
        {
            var result = new List<Movement>();
            var criteria = new NotificationInfoSelectByFunctionCriteria
            {
                FromDate = dateService.FromDate,
                ToDate = dateService.ToDate,
                ActionIDs = "1000,1002",
                NotifiedByFunctionFlag = (int)notifiedByFunction,
            };

            var rawXMLData = repositoryService.QueryRawData<RawData>(criteria);
            rawXMLData.ToList().ForEach(rawData =>
            {
                var xml = rawData.NotificationInfo;
                var movementRaw = xmlService.Parser<MovementRaw>(xml);
                var movement = transformDataService.TransformMovement(movementRaw, rawData);
                result.Add(movement);
            });

            var customerMovements = result.OrderBy(s => s.GroupHeadline).ToList();
            UpdateNumberOfGroupHeadline(customerMovements);

            return customerMovements;
        }

        private List<DirectCustomer> QueryDirectCustomer(NotifiedByFunction notifiedByFunction)
        {
            var result = new List<DirectCustomer>();
            var criteria = new NotificationInfoSelectByFunctionCriteria
            {
                FromDate = dateService.FromDate,
                ToDate = dateService.ToDate,
                ActionIDs = "600,601,602",
                NotifiedByFunctionFlag = (int)notifiedByFunction,
            };

            var rawXMLData = repositoryService.QueryRawData<RawData>(criteria);
            rawXMLData.ToList().ForEach(rawData =>
            {
                var xml = rawData.NotificationInfo;
                var directCustomerRaw = xmlService.Parser<DirectCustomerRaw>(xml);
                var directCustomer = transformDataService.TransformDirectCustomer(directCustomerRaw, rawData);
                result.Add(directCustomer);
            });

            return result;
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