using System;
using Fanex.Service.Core;

namespace TopWinningAgent.Business.Services
{
    public interface DateRangeService
    {
        DateTime FromDate(int numberOfWeekInPast);

        DateTime ToDate();
    }

    public class DateRangeServiceImp : DateRangeService
    {
        public DateTime FromDate(int numberOfWeekInPast)
        {
            var dayOfWeek = DayOfWeek();
            var numberOfDays = -dayOfWeek - ((numberOfWeekInPast * 7) - 1);
            return SystemTime.Now.AddDays(numberOfDays);
        }

        public DateTime ToDate()
        {
            var dayOfWeek = DayOfWeek();
            return SystemTime.Now.AddDays(-dayOfWeek);
        }

        private static int DayOfWeek()
        {
            var date = SystemTime.Now;
            return (int)date.DayOfWeek;
        }
    }
}