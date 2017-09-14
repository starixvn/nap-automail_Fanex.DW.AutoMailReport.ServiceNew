using System;
using System.Collections.Generic;

namespace TOControl.Notification.Business.Services
{
    public interface DateService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }

    public class DateServiceImp : DateService
    {
        private readonly ApplicationConfigurationService appConfig;

        public DateServiceImp(ApplicationConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        private Dictionary<bool, DateService> DateRanges => new Dictionary<bool, DateService>
        {
            { false,  new DateRangeAutoImp(appConfig) },
            { true,  new DateRangeManualImp(appConfig) },
        };

        public DateTime FromDate
        {
            get
            {
                return DateRanges[appConfig.Configurations.IsManual].FromDate;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return DateRanges[appConfig.Configurations.IsManual].ToDate;
            }
        }
    }
}