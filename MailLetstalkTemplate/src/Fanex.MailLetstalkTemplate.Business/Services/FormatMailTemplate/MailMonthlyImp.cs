using System;

namespace Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate
{
    public class MailMonthlyImp : MailFormat
    {
        private readonly DateRangeServiceProfier dateRangeServiceProfier;
        private DateRangeService date;

        public MailMonthlyImp(DateRangeServiceProfier dateRangeServiceProfier)
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
            return $"{AppConfigService.Config.FileName}_For_{date.ToDate:yyyy_MM}";
        }

        public string Header()
        {
            return $"{AppConfigService.Config.Header} For {date.ToDate:MMM yyyy}";
        }
    }
}