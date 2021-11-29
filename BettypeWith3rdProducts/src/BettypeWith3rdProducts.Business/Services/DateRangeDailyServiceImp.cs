using Fanex.Common;
using System;

namespace BettypeWith3rdProducts.Business.Services
{
    public class DateRangeDailyServiceImp : DateRangeService
    {
        public DateTime DateStart
        {
            get
            {
                var date = DateTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-01}");
            }
        }

        public DateTime DateEnd
        {
            get
            {
                var date = DateTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}