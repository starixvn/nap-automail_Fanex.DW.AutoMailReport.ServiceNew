using Fanex.ESportModelingData.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface DateTimeServiceProvider
    {
        DateTimeService Execute(Period period, DateTime beginDate);
    }

    public class DateTimeServiceProviderImp : DateTimeServiceProvider
    {
        public DateTimeService Execute(Period period, DateTime beginDate)
        {
            var dates = new Dictionary<Period, DateTimeService>
            {
                { Period.Daily, new DateTimeServiceDailyImp() },
                { Period.Weekly, new DateTimeServiceWeeklyImp() },
                { Period.Manual, new DateTimeServiceManualImp() },
                { Period.DailyHistoricalData, new DateTimeServiceDailyHistoryImp(beginDate) },
                { Period.WeeklyHistoricalData, new DateTimeServiceWeeklyHistoryImp(beginDate) }
            };

            return dates[period];
        }
    }
}