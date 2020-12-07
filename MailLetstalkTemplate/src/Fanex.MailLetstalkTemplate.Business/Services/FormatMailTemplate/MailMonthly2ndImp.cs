namespace Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate
{
    public class MailMonthly2ndImp : MailFormat
    {
        private readonly DateRangeServiceProfier dateRangeServiceProfier;
        private DateRangeService date;

        public MailMonthly2ndImp(DateRangeServiceProfier dateRangeServiceProfier)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange);
        }

        public string Subject()
        {
            return $"{AppConfigService.Config.EmailSubject} {date.FromDate:MMM yyyy}";
        }

        public string FileName()
        {
            return $"{AppConfigService.Config.FileName}_For_{date.FromDate:yyyy_MM}";
        }

        public string Header()
        {
            return $"{AppConfigService.Config.Header} For {date.FromDate:MMM yyyy}";
        }
    }
}