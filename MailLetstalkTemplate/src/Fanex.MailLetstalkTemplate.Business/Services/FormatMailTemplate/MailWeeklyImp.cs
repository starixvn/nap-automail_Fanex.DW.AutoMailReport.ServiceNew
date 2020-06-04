using System;

namespace Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate
{
    public class MailWeeklyImp : MailFormat
    {
        private readonly DateRangeServiceProfier dateRangeServiceProfier;
        private DateRangeService date;

        public MailWeeklyImp(DateRangeServiceProfier dateRangeServiceProfier)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange);
        }

        public string Subject()
        {
            return $"{AppConfigService.Config.EmailSubject} - {DateTime.Now:yyyy/MM/dd}";
        }

        public string FileName()
        {
            return $"{AppConfigService.Config.FileName}_From_{date.FromDate:yyyy_MM_dd}_To_{date.ToDate:yyyy_MM_dd}";
        }

        public string Header()
        {
            return $"{AppConfigService.Config.Header} From {date.FromDate:dd MMM yyyy} To {date.ToDate:dd MMM yyyy}";
        }
    }
}