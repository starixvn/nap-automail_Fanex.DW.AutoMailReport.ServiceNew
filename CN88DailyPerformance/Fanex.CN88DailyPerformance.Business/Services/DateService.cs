using Fanex.Service.Core;
using System;

namespace Fanex.CN88DailyPerformance.Service.Services
{
    public interface DateService
    {
        DateTime GetYesterday();
    }

    public class DateServiceImp : DateService
    {
        public DateTime GetYesterday()
        {
            return AppConfigService.AppConfig.IsManual
                            ? AppConfigService.AppConfig.Yesterday
                            : Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());
        }
    }
}
