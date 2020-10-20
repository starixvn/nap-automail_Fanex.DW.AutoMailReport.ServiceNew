using System;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public class DateTimeServiceManualImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                var now = AppConfigService.Config.FromDate;
                return Convert.ToDateTime($"{now:yyyy/MM/dd 00:00}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                var now = AppConfigService.Config.ToDate;
                return Convert.ToDateTime($"{now:yyyy/MM/dd 23:59}");
            }
        }
    }
}