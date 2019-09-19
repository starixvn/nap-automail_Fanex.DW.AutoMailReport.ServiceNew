using System;

namespace Fanex.SoccerPerformance.Business.Services
{
    public class DateServiceManualImp : DateService
    {
        public DateTime EndDate
        {
            get
            {
                return AppConfigService.AppConfig.EndDate;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return AppConfigService.AppConfig.StartDate;
            }
        }

        public DateTime Yesterday
        {
            get
            {
                return AppConfigService.AppConfig.Yesterday;
            }
        }
    }
}
