using System;
using Fanex.Common;
using Fanex.Service.Core;

namespace CustomerMovement.Business.Services
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

        public DateTime FromDate
        {
            get
            {
                var date = $"{SystemTime.Now.AddDays(-appConfig.Configurations.PeriodDate - 1):yyyy-MM-dd}";
                return Converter.To<DateTime>(date);
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = $"{SystemTime.Now.AddDays(-1):yyyy-MM-dd}";
                return Converter.To<DateTime>(date);
            }
        }
    }
}