using System;

namespace Fanex.ESportModelingData.Business.Services
{
    public class DateTimeServiceDailyHistoryImp : DateTimeService
    {
        private readonly DateTime beginDate;

        public DateTimeServiceDailyHistoryImp(DateTime beginDate)
        {
            this.beginDate = beginDate;
        }

        public DateTime DateEnd
        {
            get
            {
                return beginDate;
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