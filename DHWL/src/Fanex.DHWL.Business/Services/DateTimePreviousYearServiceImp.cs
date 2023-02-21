using System;

namespace Fanex.DHWL.Business.Services
{
    public class DateTimePreviousYearServiceImp : DateTimeService
    {
        private readonly DateTimeServiceProvider dateTimeService;

        public DateTimePreviousYearServiceImp(DateTimeServiceProvider dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public DateTime FromDate
        {
            get
            {
                var date = dateTimeService.DateTime.FromDate.AddYears(-1);
                return date;
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = dateTimeService.DateTime.ToDate.AddYears(-1);
                return date;
            }
        }
    }
}