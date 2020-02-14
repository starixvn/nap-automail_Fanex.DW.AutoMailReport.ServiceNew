using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public class DateRangeDailyServiceImp : DateRangeService
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