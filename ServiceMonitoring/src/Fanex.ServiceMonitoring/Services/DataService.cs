using System.Collections.Generic;
using System.Linq;
using Fanex.Data;
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

        public void CheckIn(ServiceMonitoringCheckInCriteria criteria)
        {
            repository.Execute(criteria);
        }
    }
}