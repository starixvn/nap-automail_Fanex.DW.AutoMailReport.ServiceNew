using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.DHWL.Business.Services
{
    public class DateTimeLastMonthServiceImp : DateTimeService
    {
        private DateTimeService dateTimeService = Date[AppConfigService.Config.ReportType];

        private static Dictionary<ReportType, DateTimeService> Date
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

        public DateTime FromDate
        {
            get
            {
                return Converter.To<DateTime>($"{dateTimeService.FromDate.AddDays(-1):yyyy/MM/01}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                return Converter.To<DateTime>($"{dateTimeService.FromDate.AddDays(-1):yyyy/MM/dd}");
            }
        }
    }
}