using Fanex.Service.Core;
using System;

namespace Fanex.TopLicensee.Service.Services
{
    public interface DateService
    {
        DateTime GetRptDate();
    }

    public class DateServiceImp : DateService
    {
        public DateTime GetRptDate()
        {
            return AppConfigService.AppConfig.IsManual
                            ? AppConfigService.AppConfig.RptDate
                            : Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());
        }
    }
}