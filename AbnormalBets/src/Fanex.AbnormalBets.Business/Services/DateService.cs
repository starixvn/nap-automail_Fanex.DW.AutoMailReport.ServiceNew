using Fanex.Service.Core;
using System;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface DateService
    {
        DateTime Date();
    }

    public class DateServiceImp : DateService
    {
        public DateTime Date()
        {
            return AppConfigService.AppConfig.IsManual
                ? AppConfigService.AppConfig.RptDate
                : Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy/MM/dd}");
        }
    }
}