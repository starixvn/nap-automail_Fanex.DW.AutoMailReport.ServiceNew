using System;

namespace Fanex.CWLGamePayoutAlert.Business.Services
{
    public class DateTimeServiceManualImp : DateTimeService
    {
        public DateTime FromGMT8Time
        {
            get
            {
                return AppConfigService.Config.FromGMT8Time;
            }
        }
    }
}