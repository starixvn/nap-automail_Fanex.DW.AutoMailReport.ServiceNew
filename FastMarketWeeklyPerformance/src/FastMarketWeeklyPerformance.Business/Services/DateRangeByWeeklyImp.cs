using System;
using Fanex.Common;
using Fanex.Service.Core;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public class DateRangeByWeeklyImp : DateRange
    {
        public DateTime FromDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-8);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}