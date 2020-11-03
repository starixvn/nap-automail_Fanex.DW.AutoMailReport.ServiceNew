using System;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public class DateTimeServiceImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                var now = DateTime.Now.AddHours(-1);
                return Convert.ToDateTime($"{now:yyyy/MM/dd HH:00}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var time = Convert.ToDateTime($"{DateTime.Now:yyyy/MM/dd HH:00}");
                var time2 = time.AddSeconds(-1).AddMilliseconds(998);
                return time2;
            }
        }
    }
}