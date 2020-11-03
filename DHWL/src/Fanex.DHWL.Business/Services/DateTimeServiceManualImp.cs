using System;

namespace Fanex.DHWL.Business.Services
{
    public class DateTimeServiceManualImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                var now = AppConfigService.Config.FromDate;
                return Convert.ToDateTime($"{now:yyyy/MM/dd}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var now = AppConfigService.Config.ToDate;
                return Convert.ToDateTime($"{now:yyyy/MM/dd}");
            }
        }
    }
}