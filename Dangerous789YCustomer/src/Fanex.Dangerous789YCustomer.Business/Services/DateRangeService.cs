using Fanex.Common;
using Fanex.Service.Core;
using System;

namespace Fanex.Dangerous789YCustomer.Business.Services
{
    public interface DateRangeService
    {
        DateTime RptDate { get; }
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

                var date = SystemTime.Now.AddHours(12);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }
    }
}