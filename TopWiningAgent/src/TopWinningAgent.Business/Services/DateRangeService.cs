using System;

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
            return DateTime.Now.AddDays(numberOfDays);
        }

        public DateTime ToDate()
        {
            var dayOfWeek = DayOfWeek();
            return DateTime.Now.AddDays(-dayOfWeek);
        }

        private static int DayOfWeek()
        {
            var date = DateTime.Now;
            return (int)date.DayOfWeek;
        }
    }
}