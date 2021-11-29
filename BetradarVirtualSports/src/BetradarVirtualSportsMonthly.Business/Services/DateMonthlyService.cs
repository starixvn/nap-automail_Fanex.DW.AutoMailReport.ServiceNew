using Fanex.Common;
using System;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public class DateMonthlyService : DateService
    {
        private const int MaxDay = 31;
        private const int MaxMonth = 12;

        public DateTime FromDate
        {
            get
            {
                var date = DateTime.Now.AddDays(-1);
                if (date.Day == MaxDay && date.Month == MaxMonth)
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
                var date = DateTime.Now.AddDays(-1);
                if (date.Day == MaxDay && date.Month == MaxMonth)
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