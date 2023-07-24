using Fanex.MailLetstalkTemplate.Business.Models;
using System.Collections.Generic;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface DateRangeServiceProfier
    {
        DateRangeService Date(DateRange dateRange);
    }

    public class DateRangeServiceProfierImp : DateRangeServiceProfier
    {
        private Dictionary<DateRange, DateRangeService> DateRanges
        {
            get
            {
                return new Dictionary<DateRange, DateRangeService>
                {
                    { DateRange.None, new DateRangeDailyServiceImp() },
                    { DateRange.Daily, new DateRangeDailyServiceImp() },
                    { DateRange.Weekly, new DateRangeWeeklyServiceImp() },
                    { DateRange.Weekly2, new DateRangeWeekly2ServiceImp() },
                    { DateRange.WeeklyTue, new DateRangeWeeklyTueServiceImp() },
                    { DateRange.Monthly, new DateRangeMonthlyServiceImp() },
                    { DateRange.Monthly2, new DateRangeMonthlyService2ndImp() },
                    { DateRange.Yesterday, new DateRangeYesterdayServiceImp() },
                    { DateRange.Manual, new DateRangeManualServiceImp() }
                };
            }
        }

        public DateRangeService Date(DateRange dateRange)
        {
            return DateRanges[dateRange];
        }
    }
}