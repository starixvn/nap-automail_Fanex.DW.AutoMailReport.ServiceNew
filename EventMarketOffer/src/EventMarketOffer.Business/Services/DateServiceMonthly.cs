using Fanex.Common;
using System;

namespace EventMarketOffer.Business.Services
{
    public class DateServiceMonthly : DateService
    {
        public DateTime DateEnd
        {
            get
            {
                return Converter.To<DateTime>($"{DateTime.Now:MMM-01-yyyy}").AddDays(-1);
            }
        }

        public DateTime DateStart
        {
            get
            {
                return DateEnd.AddDays(1).AddMonths(-1);
            }
        }
    }
}