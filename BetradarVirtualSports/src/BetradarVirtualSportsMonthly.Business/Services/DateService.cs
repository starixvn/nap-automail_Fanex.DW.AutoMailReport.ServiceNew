using System;
using System.Collections.Generic;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public interface DateService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }

    public class DateServiceImp : DateService
    {
        private readonly ApplicationConfigurationService appConfigService;

        public DateServiceImp(ApplicationConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        private Dictionary<bool, DateService> Dates => new Dictionary<bool, DateService>
        {
            { true, new DateManualService(appConfigService) },
            { false, new DateMonthlyService() }
        };

        public DateTime FromDate
        {
            get
            {
                return Dates[appConfigService.Configurations.IsManual].FromDate;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return Dates[appConfigService.Configurations.IsManual].ToDate;
            }
        }
    }
}