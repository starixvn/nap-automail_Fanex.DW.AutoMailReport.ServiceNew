using System.Collections.Generic;
using FastMarketWeeklyPerformance.Business.Models;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface DateRangeService
    {
        DateRange DateRange(ReportDateRange reportDateRange);
    }

    public class DateRangeServiceImp : DateRangeService
    {
        private static Dictionary<ReportDateRange, DateRange> DateRanges => new Dictionary<ReportDateRange, Services.DateRange>
        {
            { ReportDateRange.None, new DateRangeByWeeklyImp() },
            { ReportDateRange.Weekly, new DateRangeByWeeklyImp() },
            { ReportDateRange.Manual, new DateRangeByManualImp() }
        };

        public DateRange DateRange(ReportDateRange reportDateRange)
        {
            return DateRanges[reportDateRange];
        }
    }
}