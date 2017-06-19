using System;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface DateRange
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}