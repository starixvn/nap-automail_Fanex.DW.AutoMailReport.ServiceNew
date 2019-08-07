using System.Collections.Generic;

namespace Fanex.ScoreBox.Business.Services
{
    public interface DateProvider
    {
        DateService Date(bool isDaily);
    }

    public class DateProviderImp : DateProvider
    {
        private static Dictionary<bool, DateService> DateRanges
        {
            get
            {
                return new Dictionary<bool, DateService>
                {
                    { true, new DateDailyServiceImp() },
                    { false, new DateMonthlyServiceImp() }
                };
            }
        }

        public DateService Date(bool isDaily)
        {
            return DateRanges[isDaily];
        }
    }
}