using System;
using System.Collections.Generic;

namespace EventMarketOffer.Business.Services
{
    public interface DateService
    {
        DateTime DateStart { get; }

        DateTime DateEnd { get; }
    }

    public class DateServiceImp : DateService
    {
        private readonly AppConfigurationService appConfigService;

        public DateServiceImp(AppConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        private Dictionary<bool, DateService> Dates => new Dictionary<bool, DateService>
        {
            { false, new DateServiceMonthly() },
            { true, new DateServiceManual(appConfigService) }
        };

        public DateTime DateEnd
        {
            get
            {
                return Dates[appConfigService.Configurations.IsManual].DateEnd;
            }
        }

        public DateTime DateStart
        {
            get
            {
                return Dates[appConfigService.Configurations.IsManual].DateStart;
            }
        }
    }
}