using Fanex.Service.Core;
using System;

namespace Fanex.ESportModelingData.Business.Services
{
    public class DateTimeServiceDailyImp : DateTimeService
    {
        public DateTime DateEnd
        {
            get
            {
                return Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy-MM-dd}");
            }
        }

        public DateTime DateStart
        {
            get
            {
                return Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy-MM-dd}");
            }
        }
    }
}