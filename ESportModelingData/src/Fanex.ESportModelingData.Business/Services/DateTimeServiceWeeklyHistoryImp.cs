using System;

namespace Fanex.ESportModelingData.Business.Services
{
    public class DateTimeServiceWeeklyHistoryImp : DateTimeService
    {
        private readonly DateTime beginDate;

        public DateTimeServiceWeeklyHistoryImp(DateTime beginDate)
        {
            this.beginDate = beginDate;
        }

        public DateTime DateEnd
        {
            get
            {
                const int Week = 6;
                return DateStart.AddDays(Week);
            }
        }

        public DateTime DateStart
        {
            get
            {
                return beginDate;
            }
        }
    }
}