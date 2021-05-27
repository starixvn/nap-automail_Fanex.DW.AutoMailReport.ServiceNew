using System;

namespace Fanex.CWLGamePayoutAlert.Business.Services
{
    public class DateTimeServiceImp : DateTimeService
    {
        public DateTime FromGMT8Time
        {
            get
            {
                var now = DateTime.Now;
                var gmt8 = now.AddHours(11);
                return gmt8;
            }
        }
    }
}