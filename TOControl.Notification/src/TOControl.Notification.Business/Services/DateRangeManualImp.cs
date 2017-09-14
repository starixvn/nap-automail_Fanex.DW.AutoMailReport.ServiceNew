using System;

namespace TOControl.Notification.Business.Services
{
    public class DateRangeManualImp : DateService
    {
        private readonly ApplicationConfigurationService appConfig;

        public DateRangeManualImp(ApplicationConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        public DateTime FromDate
        {
            get
            {
                return appConfig.Configurations.FromDate;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return appConfig.Configurations.ToDate;
            }
        }
    }
}