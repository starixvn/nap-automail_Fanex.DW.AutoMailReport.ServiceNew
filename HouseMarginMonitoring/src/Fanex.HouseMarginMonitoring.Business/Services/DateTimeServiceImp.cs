using System;

namespace Fanex.HouseMarginMonitoring.Business.Services
{
    public class DateTimeServiceImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                var now = DateTime.Now;
                return Convert.ToDateTime($"{now:yyyy/MM/dd}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var time = Convert.ToDateTime($"{DateTime.Now:yyyy/MM/dd}");
                return time;
            }
        }
    }
}