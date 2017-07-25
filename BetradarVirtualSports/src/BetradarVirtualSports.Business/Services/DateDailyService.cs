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
                return SystemTime.Now.AddDays(-1);
            }
        }
    }
}