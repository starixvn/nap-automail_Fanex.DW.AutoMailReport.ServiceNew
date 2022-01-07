using Fanex.MailLetstalkTemplate.Business.Models;
using System.Collections.Generic;

namespace Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate
{
    public interface MailFormatProvider
    {
        MailFormat Format(DateRange dateRange);
    }

    public class MailFormatProviderImp : MailFormatProvider
    {
        private readonly DateRangeServiceProfier dateRangeServiceProfier;

        public MailFormatProviderImp(DateRangeServiceProfier dateRangeServiceProfier)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
        }

        private Dictionary<DateRange, MailFormat> DateRanges
        {
            get
            {
                return new Dictionary<DateRange, MailFormat>
                {
                    { DateRange.None, new MailDailyImp(dateRangeServiceProfier) },
                    { DateRange.Daily, new MailWeeklyImp(dateRangeServiceProfier) },
                    { DateRange.Weekly, new MailWeeklyImp(dateRangeServiceProfier) },
                    { DateRange.Weekly2, new MailWeeklyImp(dateRangeServiceProfier) },
                    { DateRange.Monthly, new MailMonthlyImp(dateRangeServiceProfier) },
                    { DateRange.Monthly2, new MailMonthly2ndImp(dateRangeServiceProfier) },
                    { DateRange.Yesterday, new MailDailyImp(dateRangeServiceProfier) },
                    { DateRange.Manual, new MailWeeklyImp(dateRangeServiceProfier) }
                };
            }
        }

        public MailFormat Format(DateRange dateRange)
        {
            return DateRanges[dateRange];
        }
    }
}