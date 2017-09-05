using System;
using Fanex.Common;
using Fanex.Service.Core;

namespace EventMarketOffer.Business.Services
{
    public class DateServiceMonthly : DateService
    {
        public DateTime DateEnd
        {
            get
            {
                return Converter.To<DateTime>($"{SystemTime.Now:MMM-01-yyyy}").AddDays(-1);
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