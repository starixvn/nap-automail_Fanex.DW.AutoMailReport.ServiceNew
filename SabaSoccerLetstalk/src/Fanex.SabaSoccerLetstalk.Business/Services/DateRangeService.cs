using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface DateRangeService
    {
        DateTime RptDate { get; }

        DateTime Today { get; }
    }

    public class DateRangeServiceImp : DateRangeService
    {
        public DateTime RptDate
        {
            get
            {
                if (AppConfigService.Config.IsManual)
                {
                    return AppConfigService.Config.RptDate;
                }

                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }

        public DateTime Today
        {
            get
            {
                if (AppConfigService.Config.IsManual)
                {
                    return AppConfigService.Config.RptDate;
                }

                var date = SystemTime.Now.AddMinutes(-30);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}