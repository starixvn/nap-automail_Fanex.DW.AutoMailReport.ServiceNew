using System;

namespace Fanex.SoccerNonLive.Business.Services
{
    public class DateTimeDailyServiceImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                return DateTime.Today.AddDays(-1);
            }
        }

        public DateTime ToDate
        {
            get
            {
                return DateTime.Today.AddDays(-1);
            }
        }
    }
}