using Fanex.SoccerNonLive.Business.Models;
using System.Collections.Generic;

namespace Fanex.SoccerNonLive.Business.Services
{
    public interface DateTimeServiceProvider
    {
        DateTimeService DateTime { get; }
    }

    public class DateTimeServiceProviderImp : DateTimeServiceProvider
    {
        private Dictionary<ReportType, DateTimeService> Date
        {
            get
            {
                return new Dictionary<ReportType, DateTimeService>
                {
                    { ReportType.Manual, new DateTimeServiceManualImp()},
                    { ReportType.Daily, new DateTimeDailyServiceImp()},
                };
            }
        }

        public DateTimeService DateTime
        {
            get
            {
                return Date[AppConfigService.Config.ReportType];
            }
        }
    }
}