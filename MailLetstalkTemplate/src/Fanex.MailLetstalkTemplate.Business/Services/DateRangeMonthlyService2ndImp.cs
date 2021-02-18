using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public class DateRangeMonthlyService2ndImp : DateRangeService
    {
        public DateTime FromDate
        {
            get
            {
                var date = SystemTime.Now;
                return Converter.To<DateTime>($"{date.AddMonths(-1):yyyy-MM-02}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = SystemTime.Now;
                return Converter.To<DateTime>($"{date:yyyy-MM-01}");
            }
        }
    }
}