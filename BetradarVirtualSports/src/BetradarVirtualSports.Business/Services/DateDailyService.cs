using System;
using Fanex.Service.Core;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public class DateDailyService : DateService
    {
        public DateTime Date
        {
            get
            {
                var date = $"{SystemTime.Now.AddDays(-1):yyyy/MM/dd}";
                return Convert.ToDateTime(date);
            }
        }
    }
}