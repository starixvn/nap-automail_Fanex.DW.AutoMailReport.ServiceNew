using System;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public class DateDailyService : DateService
    {
        public DateTime Date
        {
            get
            {
                var date = $"{DateTime.Now.AddDays(-1):yyyy/MM/dd}";
                return Convert.ToDateTime(date);
            }
        }
    }
}