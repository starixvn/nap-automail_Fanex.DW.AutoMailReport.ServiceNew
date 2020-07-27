using Fanex.Service.Core;
using System;

namespace Fanex.TopLicensee.Service.Services
{
    public interface DateService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }

    public class DateServiceImp : DateService
    {
        public DateTime FromDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return AppConfigService.AppConfig.IsManual
                           ? AppConfigService.AppConfig.FromDate
                           : Convert.ToDateTime($"{date:yyyy-MM-01}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var date = SystemTime.Now.AddDays(-1);
                return AppConfigService.AppConfig.IsManual
                           ? AppConfigService.AppConfig.ToDate
                           : Convert.ToDateTime($"{date:yyyy-MM-dd}");
            }
        }
    }
}