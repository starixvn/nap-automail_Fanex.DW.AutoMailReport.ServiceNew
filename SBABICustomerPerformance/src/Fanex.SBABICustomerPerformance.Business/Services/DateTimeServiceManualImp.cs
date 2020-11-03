using System;

namespace Fanex.SBABICustomerPerformance.Business.Services
{
    public class DateTimeServiceManualImp : DateTimeService
    {
        public DateTime WinlostDate
        {
            get
            {
                var now = AppConfigService.Config.FromDate;
                return Convert.ToDateTime($"{now:yyyy/MM/dd}");
            }
        }
    }
}