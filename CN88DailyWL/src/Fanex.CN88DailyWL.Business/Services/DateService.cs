using Fanex.Service.Core;
using System;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface DateService
    {
        DateTime WinLostDate { get; }
    }

    public class DateServiceImp : DateService
    {
        public DateTime WinLostDate
        {
            get
            {
                var wldate = AppConfigService.AppConfig.IsManual
                    ? AppConfigService.AppConfig.WinLostDate
                    : Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy/MM/dd}");

                return wldate;
            }
        }
    }
}