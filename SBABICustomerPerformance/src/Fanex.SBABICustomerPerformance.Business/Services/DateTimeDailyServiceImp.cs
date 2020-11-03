using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.SBABICustomerPerformance.Business.Services
{
    public class DateTimeDailyServiceImp : DateTimeService
    {
        public DateTime WinlostDate
        {
            get
            {
                var date = SystemTime.Now;
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}