using Fanex.Service.Core;
using System;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public class DateRangeWeeklyTueServiceImp : DateRangeService
    {
        public DateTime FromDate
        {
            get
            {
                DateTime dateStart;
                int diff = GetDateDiff();
                var dateEnd = Today().AddDays(-diff);
                const int Week = 6;
                dateStart = dateEnd.AddDays(-Week);

                return dateStart;
            }
        }

        public DateTime ToDate
        {
            get
            {
                const int Week = 6;
                return FromDate.AddDays(Week);
            }
        }

        private static DateTime Today()
        {
            var today = $"{SystemTime.Now:yyyy/MM/dd}";
            return Convert.ToDateTime(today);
        }

        private static int GetDateDiff()
        {
            var today = Today();
            const int MondayValue = (int)DayOfWeek.Tuesday;
            var todayValue = (int)today.DayOfWeek;
            var diff = todayValue - MondayValue + 1;

            return diff;
        }
    }
}