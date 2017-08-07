using System;
using System.Collections.Generic;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public interface DateService
    {
        DateTime Date { get; }
    }

    public class DateServiceImp : DateService
    {
        private readonly ApplicationConfigurationService appConfigService;

        public DateServiceImp(ApplicationConfigurationService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        public DateTime Date
        {
            get
            {
                var dateService = new Dictionary<bool, DateService>
                {
                    { true, new DateManualService(appConfigService) },
                    { false, new DateDailyService() }
                };

                return dateService[appConfigService.Configurations.IsManual].Date;
            }
        }
    }
}