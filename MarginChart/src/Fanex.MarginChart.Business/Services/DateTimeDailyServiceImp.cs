using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.MarginChart.Business.Services
{
    public class DateTimeDailyServiceImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-01}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}