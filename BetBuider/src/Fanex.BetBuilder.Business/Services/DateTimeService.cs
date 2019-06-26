using System;

namespace Fanex.BetBuilder.Business.Services
{
    public interface DateTimeService
    {
        DateTime FromDate();

        DateTime ToDate();
    }

    public class DateTimeServiceImp : DateTimeService
    {
        public DateTime FromDate()
        {
            var wldate = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.WinlostDate : DateTime.Today.AddDays(-1);
            wldate = wldate.AddDays(-(wldate.Day - 1)).AddMonths(-1);

            return wldate;
        }

        public DateTime ToDate()
        {
            var wldate = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.WinlostDate : DateTime.Today.AddDays(-1);
            wldate = wldate.AddDays(-wldate.Day);

            return wldate;
        }
    }
}