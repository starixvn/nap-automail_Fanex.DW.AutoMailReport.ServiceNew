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
        private readonly ApplicationConfigurationService appConfig;

        public DataServiceImp(
            DateService dateService,
            XMLService xmlService,
            TransformDataService transformDataService,
            TOControlRepositoryService repositoryService,
            ApplicationConfigurationService appConfig)
        {
            this.dateService = dateService;
            this.xmlService = xmlService;
            this.transformDataService = transformDataService;
            this.repositoryService = repositoryService;
            this.appConfig = appConfig;

            ValidIds = new Dictionary<int, int>();
            InvalidIds = new Dictionary<int, int>();
        }

        private Dictionary<int, int> ValidIds
        {
            get;
        }

        private Dictionary<int, int> InvalidIds
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
                    ValidIds.Add(rawData.Id, groupHeadline.Currency);
                    result.Add(groupHeadline);
                }
                else
                {
                    InvalidIds.Add(rawData.Id, groupHeadline.Currency);
                }
            });

            var testCurrency = result.Where(s => s.Currency == 20).ToList();
            var realCurrency = result.Where(s => s.Currency != 20).ToList();
            var data = appConfig.Configurations.IsRealCurrency ? realCurrency : testCurrency;

            return data;
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
                    ValidIds.Add(rawData.Id, movement.Currency);
                    result.Add(movement);
                }
                else
                {
                    InvalidIds.Add(rawData.Id, movement.Currency);
                }
            });

            var customerMovements = result.OrderBy(s => s.GroupHeadline).ToList();
            UpdateNumberOfGroupHeadline(customerMovements);

            var testCurrency = customerMovements.Where(s => s.Currency == 20).ToList();
            var realCurrency = customerMovements.Where(s => s.Currency != 20).ToList();
            var data = appConfig.Configurations.IsRealCurrency ? realCurrency : testCurrency;

            return data;            
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
                ValidIds.Add(rawData.Id, directCustomer.Currency);
            });

            var testCurrency = result.Where(s => s.Currency == 20).ToList();
            var realCurrency = result.Where(s => s.Currency != 20).ToList();
            var data = appConfig.Configurations.IsRealCurrency ? realCurrency : testCurrency;

            return data;
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
            var idtestCurrency = ValidIds.Where(s => s.Value == 20).Select(s => s.Key);
            var idrealCurrency = ValidIds.Where(s => s.Value != 20).Select(s => s.Key);
            var ids = appConfig.Configurations.IsRealCurrency ? idrealCurrency : idtestCurrency;

            var command = new NotificationInfoUpdateByFunctionCommand
            {
                NotificationIdList = string.Join(",", ids),
                IsNotifiedValue = 1,
                NotifiedByFunctionFlag = (int)notifiedByFunction
            };

            repositoryService.UpdateNotificationStatus(command);
        }

        private void UpdateInvalidIds(NotifiedByFunction notifiedByFunction)
        {
            var idtestCurrency = InvalidIds.Where(s => s.Value == 20).Select(s => s.Key);
            var idrealCurrency = InvalidIds.Where(s => s.Value != 20).Select(s => s.Key);
            var ids = appConfig.Configurations.IsRealCurrency ? idrealCurrency : idtestCurrency;

            var command = new NotificationInfoUpdateByFunctionCommand
            {
                NotificationIdList = string.Join(",", ids),
                IsNotifiedValue = 2,
                NotifiedByFunctionFlag = (int)notifiedByFunction
            };

            repositoryService.UpdateNotificationStatus(command);
        }
    }
}