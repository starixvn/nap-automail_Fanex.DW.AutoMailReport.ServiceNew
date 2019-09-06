using Fanex.Service.Core;
using System;

namespace Fanex.SoccerPerformance.Business.Services
{
    public class DateServiceAutoImp : DateService
    {
        public DateTime EndDate
        {
            get
            {
                return Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy/MM/dd}");
            }
        }

        public DateTime StartDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return Convert.ToDateTime($"{date:yyyy/MM/01}");
            }
        }

        public DateTime Yesterday
        {
            get
            {
                return Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy/MM/dd}");
            }
        }
    }
}
