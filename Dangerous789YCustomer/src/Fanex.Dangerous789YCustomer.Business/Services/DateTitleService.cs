namespace Fanex.Dangerous789YCustomer.Business.Services
{
    public interface DateTitleService
    {
        string DateGroup1();

        string DateGroup2();

        string DateGroup3();
    }

    public class DateTitleServiceImp : DateTitleService
    {
        private readonly DateRangeService dateRangeService;

        public DateTitleServiceImp(DateRangeService dateRangeService)
        {
            this.dateRangeService = dateRangeService;
        }

        public string DateGroup1()
        {
            var date1 = dateRangeService.RptDate.AddDays(-1);
            var date2 = dateRangeService.RptDate;

            return $"Today ({date1:dd MMM} - {date2:dd MMM})";
        }

        public string DateGroup2()
        {
            var date1 = dateRangeService.RptDate;
            if (dateRangeService.RptDate.Day == 1 || dateRangeService.RptDate.Day == 2)
            {
                date1 = dateRangeService.RptDate.AddMonths(-1);
            }

            var date2 = dateRangeService.RptDate;

            return $"MTD ({date1:01 MMM} - {date2:dd MMM})";
        }

        public string DateGroup3()
        {
            var date1 = dateRangeService.RptDate.AddMonths(-2);
            if (dateRangeService.RptDate.Day == 1 || dateRangeService.RptDate.Day == 2)
            {
                date1 = dateRangeService.RptDate.AddMonths(-3);
            }

            var date2 = dateRangeService.RptDate;

            return $"3 Months ({date1:01 MMM} - {date2:dd MMM})";
        }
    }
}