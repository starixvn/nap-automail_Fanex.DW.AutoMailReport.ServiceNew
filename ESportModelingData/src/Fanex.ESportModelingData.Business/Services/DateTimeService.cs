using System;
using Fanex.Service.Core;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface DateTimeService
    {
        DateTime DateStart();

        DateTime DateEnd();
    }

    public class DateTimeServiceImp : DateTimeService
    {
        public DateTime DateEnd()
        {
            const int Week = 6;

            return DateStart().AddDays(Week);
        }

        public DateTime DateStart()
        {
            DateTime dateStart;
            int diff = GetDateDiff();
            var dateEnd = Today().AddDays(-diff);
            const int Week = 6;

            dateStart = dateEnd.AddDays(-Week);

            if (AppConfigService.AppConfig.IsManual && dateStart <= AppConfigService.AppConfig.DateStart)
            {
                dateStart = dateStart.AddDays(Week + 1);
            }

            return dateStart;
        }

        private static DateTime Today()
        {
            if (AppConfigService.AppConfig.IsManual)
            {
                return AppConfigService.AppConfig.DateStart;
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