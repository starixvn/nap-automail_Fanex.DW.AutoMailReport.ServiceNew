using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Fanex.ServiceMonitoring.Criterias;
using Fanex.ServiceMonitoring.Models;

namespace Fanex.ServiceMonitoring.Services
{
    public interface DataService
    {
        List<ServiceModel> QueryService();

        void WriteLogServiceRestartTime(List<ServiceModel> services);

        void CheckIn(ServiceMonitoringCheckInCriteria criteria);

        List<MailTaskModel> QueryMailTasks();

        List<MailTaskDetailModel> QuerySchedules();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly CommonService commonService;

        public DataServiceImp(IDynamicRepository repository, CommonService commonService)
        {
            this.repository = repository;
            this.commonService = commonService;
        }

        public List<ServiceModel> QueryService()
        {
            var services = repository.Fetch<ServiceModel>(new QueryServiceCriteria()).ToList();
            var ipServer = commonService.GetLocalIP();
            var servicesByIP = services.Where(s => s.IPServer == ipServer).ToList();
            return servicesByIP;
        }

        public void WriteLogServiceRestartTime(List<ServiceModel> services)
        {
            var restartedServices = services.Where(s => s.IsRestartSuccessfully);
            foreach (var service in restartedServices)
            {
                var criteria = new LogServiceRestartTimeCriteria { ServiceId = service.ServiceId };
                repository.Execute(criteria);
            }
        }

        public List<MailTaskModel> QueryMailTasks()
        {
            var fromDate = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.FromDate : DateTime.Today;
            var toDate = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.ToDate : DateTime.Today;
            var criteria = new MailReportCriteria { FromDate = fromDate, ToDate = toDate, Status = -1, SiteId = -1 };
            return repository.Fetch<MailTaskModel>(criteria).ToList();
        }

        public void CheckIn(ServiceMonitoringCheckInCriteria criteria)
        {
            repository.Execute(criteria);
        }

        public List<MailTaskDetailModel> QuerySchedules()
        {
            var criteria = new GetAllScheduleCriteria { SiteId = -1 };
            return repository.Fetch<MailTaskDetailModel>(criteria).ToList();
        }
    }
}