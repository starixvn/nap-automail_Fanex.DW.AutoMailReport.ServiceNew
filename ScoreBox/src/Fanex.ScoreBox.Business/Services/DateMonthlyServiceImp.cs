using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.ScoreBox.Business.Services
{
    public class DateMonthlyServiceImp : DateService
    {
        private const int MaxDay = 31;
        private const int MaxMonth = 12;

        public DateTime FromDate
        {
            get
            {
                if (AppConfigService.AppConfig.IsManual)
                {
                    return AppConfigService.AppConfig.FromDate;
                }

                var date = SystemTime.Now.AddDays(-1);
                if (date.Day == MaxDay && date.Month == MaxMonth)
                {
                    return Converter.To<DateTime>($"{date:yyyy-MM-01}");
                }

                return Converter.To<DateTime>($"{date.AddMonths(-1):yyyy-MM-01}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                if (AppConfigService.AppConfig.IsManual)
                {
                    return AppConfigService.AppConfig.ToDate;
                }

                var date = SystemTime.Now.AddDays(-1);
                if (date.Day == MaxDay && date.Month == MaxMonth)
                {
                    return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
                }

                date = Converter.To<DateTime>($"{date:yyyy-MM-dd}");
                var numOfDays = date.Day;
                return date.AddDays(-numOfDays);
            }
        }
    }
}