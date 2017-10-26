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
        Tuple<IEnumerable<GroupHeadline>, IEnumerable<CustomerMovement>, IEnumerable<DirectCustomer>> Query(NotifiedByFunction notifiedByFunction);

        void Update(NotifiedByFunction notifiedByFunction);
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

            ValidIds = new List<int>();
            InvalidIds = new List<int>();
        }

        private List<int> ValidIds
        {
            get;
        }

        private List<int> InvalidIds
        {
            get;
        }

        public Tuple<IEnumerable<GroupHeadline>, IEnumerable<CustomerMovement>, IEnumerable<DirectCustomer>> Query(NotifiedByFunction notifiedByFunction)
        {
            var customersExistingSetting = QueryCustomersExistingSetting(notifiedByFunction);
            var groupHeadlines = QueryGroupHeadline(notifiedByFunction, customersExistingSetting);
            var customerMovements = QueryCustomerMovement(notifiedByFunction, customersExistingSetting);
            var newDirectCustomers = QueryDirectCustomer(notifiedByFunction);

            return new Tuple<IEnumerable<GroupHeadline>, IEnumerable<CustomerMovement>, IEnumerable<DirectCustomer>>(groupHeadlines, customerMovements, newDirectCustomers);
        }

        private List<GroupHeadline> QueryGroupHeadline(NotifiedByFunction notifiedByFunction, List<CustomerExistingSetting> customersExistingSetting)
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

                var count = customersExistingSetting.Count(c => c.CustId == groupHeadlineRaw.CustId);
                if (count > 0)
                {
                    ValidIds.Add(rawData.Id);
                    result.Add(groupHeadline);
                }
                else
                {
                    InvalidIds.Add(rawData.Id);
                }
            });

            return result;
        }

        private List<CustomerMovement> QueryCustomerMovement(NotifiedByFunction notifiedByFunction, List<CustomerExistingSetting> customersExistingSetting)
        {
            var result = new List<CustomerMovement>();
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
                var movementRaw = xmlService.Parser<CustomerMovementRaw>(xml);
                var movement = transformDataService.TransformMovement(movementRaw, rawData);

                var count = customersExistingSetting.Count(c => c.GroupId == movementRaw.GroupId);
                if (count > 0)
                {
                    ValidIds.Add(rawData.Id);
                    result.Add(movement);
                }
                else
                {
                    InvalidIds.Add(rawData.Id);
                }
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
                ValidIds.Add(rawData.Id);
            });

            return result;
        }

        private static void UpdateNumberOfGroupHeadline(IEnumerable<CustomerMovement> customerMovements)
        {
            foreach (var m in customerMovements)
            {
                m.Count = customerMovements.Count(c => c.GroupHeadline == m.GroupHeadline);
            }
        }

        private List<CustomerExistingSetting> QueryCustomersExistingSetting(NotifiedByFunction notifiedByFunction)
        {
            var criteria = new EmailNotificationGlobalSettingSelectCriteria
            {
                NotifiedByFunctionFlag = (int)notifiedByFunction
            };

            return repositoryService.QueryRawData<CustomerExistingSetting>(criteria).ToList();
        }

        public void Update(NotifiedByFunction notifiedByFunction)
        {
            UpdateValidIds(notifiedByFunction);
            UpdateInvalidIds(notifiedByFunction);
        }

        private void UpdateValidIds(NotifiedByFunction notifiedByFunction)
        {
            var command = new NotificationInfoUpdateByFunctionCommand
            {
                NotificationIdList = string.Join(",", ValidIds),
                IsNotifiedValue = 1,
                NotifiedByFunctionFlag = (int)notifiedByFunction
            };

            repositoryService.UpdateNotificationStatus(command);
        }

        private void UpdateInvalidIds(NotifiedByFunction notifiedByFunction)
        {
            var command = new NotificationInfoUpdateByFunctionCommand
            {
                NotificationIdList = string.Join(",", InvalidIds),
                IsNotifiedValue = 2,
                NotifiedByFunctionFlag = (int)notifiedByFunction
            };

            repositoryService.UpdateNotificationStatus(command);
        }
    }
}