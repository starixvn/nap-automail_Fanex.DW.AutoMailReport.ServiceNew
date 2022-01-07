using Fanex.Service.Core;
using System;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public class DateRangeWeekly2ServiceImp : DateRangeService
    {
        public DateTime FromDate
        {
            get
            {
                DateTime dateStart;
                int diff = GetDateDiff();
                var dateEnd = Today().AddDays(-diff);
                const int Week = 13;
                dateStart = dateEnd.AddDays(-Week);

                return dateStart;
            }
        }

        public DateTime ToDate
        {
            get
            {
                int diff = 13;
                return FromDate.AddDays(diff);
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
            const int MondayValue = (int)DayOfWeek.Monday;
            var todayValue = (int)today.DayOfWeek;
            var diff = todayValue - MondayValue + 1;

            return diff;
        }
    }
}