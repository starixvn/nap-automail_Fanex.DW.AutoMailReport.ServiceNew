using System;

namespace EventMarketOffer.Business.Services
{
    public class DateServiceManual : DateService
    {
        private readonly AppConfigurationService appConfig;

        public DateServiceManual(AppConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        public DateTime DateEnd
        {
            get
            {
                return appConfig.Configurations.DateEnd;
            }
        }

        public DateTime DateStart
        {
            get
            {
                return appConfig.Configurations.DateStart;
            }
        }
    }
}