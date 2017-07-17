using System;
using Fanex.Common;
using Fanex.Service.Core;

namespace BettypeWith3rdProducts.Business.Services
{
    public class DateRangeDailyServiceImp : DateRangeService
    {
        public DateTime DateStart
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-01}");
            }
        }

        public DateTime DateEnd
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}