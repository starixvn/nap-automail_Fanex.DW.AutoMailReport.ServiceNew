using Fanex.Service.Core;
using System;

namespace Fanex.ESportDataMining.Business.Services
{
    public interface DateTimeService
    {
        DateTime FromDate();

        DateTime ToDate();
    }

    public class DateTimeServiceImp : DateTimeService
    {
        public DateTime ToDate()
        {
            if (AppConfigService.AppConfig.IsManual)
            {
                return AppConfigService.AppConfig.ToDate;
            }

            const int Week = 6;
            return FromDate().AddDays(Week);
        }

        public DateTime FromDate()
        {
            if (AppConfigService.AppConfig.IsManual)
            {
                return AppConfigService.AppConfig.FromDate;
            }

            DateTime dateStart;
            int diff = GetDateDiff();
            var dateEnd = Today().AddDays(-diff);
            const int Week = 6;
            dateStart = dateEnd.AddDays(-Week);

            return dateStart;
        }

        private static DateTime Today()
        {
            if (AppConfigService.AppConfig.IsManual)
            {
                return AppConfigService.AppConfig.ToDate;
            }

            var today = $"{SystemTime.Now:yyyy/MM/dd}";
            return Convert.ToDateTime(today);
        }

        private static int GetDateDiff()
        {
            var today = Today();
            const int MondayValue = (int)DayOfWeek.Monday;
            var todayValue = (int)today.DayOfWeek;
            var diff = todayValue - MondayValue + 1;

            return diff;
        }
    }
}