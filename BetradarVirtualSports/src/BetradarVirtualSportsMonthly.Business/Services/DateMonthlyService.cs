using System;
using Fanex.Common;
using Fanex.Service.Core;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public class DateMonthlyService : DateService
    {
        public DateTime FromDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                if (date.Day == 31 && date.Month == 12)
                {
                    return Converter.To<DateTime>($"{date:yyyy-MM-01}");
                }

                return Converter.To<DateTime>($"{date.AddMonths(-1):yyyy-MM-01}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                if (date.Day == 31 && date.Month == 12)
                {
                    return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
                }

                date = Converter.To<DateTime>($"{date:yyyy-MM-dd}");
                var numOfDays = date.Day;
                return date.AddDays(-numOfDays);
            }
        }
    }
}