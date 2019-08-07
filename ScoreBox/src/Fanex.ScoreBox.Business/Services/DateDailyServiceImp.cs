using Fanex.Service.Core;
using System;

namespace Fanex.ScoreBox.Business.Services
{
    public class DateDailyServiceImp : DateService
    {
        public DateTime FromDate
        {
            get
            {
                if (AppConfigService.AppConfig.IsManual)
                {
                    return AppConfigService.AppConfig.FromDate;
                }

                return Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());
            }
        }

        public DateTime ToDate
        {
            get
            {
                if (AppConfigService.AppConfig.IsManual)
                {
                    return AppConfigService.AppConfig.ToDate;
                }

                return Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());
            }
        }
    }
}