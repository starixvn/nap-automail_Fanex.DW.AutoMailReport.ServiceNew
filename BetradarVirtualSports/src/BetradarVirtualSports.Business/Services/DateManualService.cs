using System;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public class DateManualService : DateService
    {
        private readonly ApplicationConfigurationService appConfigService;

        public DateManualService(ApplicationConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        public DateTime Date
        {
            get
            {
                return appConfigService.Configurations.Date;
            }
        }
    }
}