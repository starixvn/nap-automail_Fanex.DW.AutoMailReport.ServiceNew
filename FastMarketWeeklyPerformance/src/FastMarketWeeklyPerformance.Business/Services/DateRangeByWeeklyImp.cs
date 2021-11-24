using Fanex.Common;
using System;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public class DateRangeByWeeklyImp : DateRange
    {
        public DateTime FromDate
        {
            get
            {
                var date = DateTime.Now.AddDays(-7);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = DateTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}