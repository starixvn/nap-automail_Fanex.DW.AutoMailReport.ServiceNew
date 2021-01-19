using Fanex.MarginChart.Business.Models;
using System.Collections.Generic;

namespace Fanex.MarginChart.Business.Services
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
                    { ReportType.Monthly, new DateTimeMonthlyServiceImp()}
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