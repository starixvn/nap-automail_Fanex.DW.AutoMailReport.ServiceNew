using System;

namespace Fanex.TopSportbookAgents.Business.Services
{
    public interface DateTitleService
    {
        string DateGroup1();

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
            var date1 = dateRangeService.RptDate.AddDays(-2);
            var date2 = dateRangeService.RptDate;

            return $"{date1:yyyyMMdd} - {date2:yyyyMMdd}";
        }

        public string DateGroup3()
        {
            var date = Convert.ToDateTime($"{dateRangeService.RptDate:yyyy/MM/01}");
            var date1 = date.AddMonths(-2);
            var date2 = dateRangeService.RptDate;

            return $"{date1:yyyyMMdd} - {date2:yyyyMMdd}";
        }
    }
}