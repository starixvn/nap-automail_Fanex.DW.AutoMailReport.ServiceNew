using System;
using System.Collections.Generic;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface DateRangeService
    {
        DateTime DateStart { get; }

        DateTime DateEnd { get; }
    }

    public class DateRangeServiceImp : DateRangeService
    {
        private readonly ApplicationConfigurationService appConfig;

        public DateRangeServiceImp(ApplicationConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        private static Dictionary<bool, DateRangeService> DateRanges => new Dictionary<bool, DateRangeService>
        {
            { false, new DateRangeDailyServiceImp() },
            { true, new DateRangeManualServiceImp() }
        };

        public DateTime DateEnd
        {
            get
            {
                return DateRanges[appConfig.IsManual].DateEnd;
            }
        }

        public DateTime DateStart
        {
            get
            {
                return DateRanges[appConfig.IsManual].DateStart;
            }
        }
    }
}