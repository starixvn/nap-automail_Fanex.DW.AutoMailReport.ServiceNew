using System.Collections.Generic;
using BettypeWith3rdProducts.Business.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface DateRangeServiceProvider
    {
        DateRangeService Date(DateRange dateRange);
    }

    public class DateRangeServiceProviderImp : DateRangeServiceProvider
    {
        private static Dictionary<DateRange, DateRangeService> DateRanges => new Dictionary<DateRange, DateRangeService>
        {
            { DateRange.None, new DateRangeDailyServiceImp() },
            { DateRange.Daily, new DateRangeDailyServiceImp() },
            { DateRange.Manual, new DateRangeManualServiceImp() }
        };

        public DateRangeService Date(DateRange dateRange)
        {
            return DateRanges[dateRange];
        }
    }
}