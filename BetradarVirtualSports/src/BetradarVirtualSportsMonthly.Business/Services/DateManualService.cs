using System;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public class DateManualService : DateService
    {
        private readonly ApplicationConfigurationService appConfigService;

        public DateManualService(ApplicationConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        public DateTime FromDate
        {
            get
            {
                return appConfigService.Configurations.FromDate;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return appConfigService.Configurations.ToDate;
            }
        }
    }
}