using System;

namespace Fanex.SoccerNonLive.Business.Services
{
    public class DateTimeServiceManualImp : DateTimeService
    {
        public DateTime FromDate
        {
            get
            {
                return AppConfigService.Config.FromDate;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return AppConfigService.Config.ToDate;
            }
        }
    }
}