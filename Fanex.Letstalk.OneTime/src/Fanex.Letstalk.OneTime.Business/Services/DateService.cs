using System;

namespace Fanex.Letstalk.OneTime.Business.Services
{
    public interface DateService
    {
        DateTime Date { get; }
    }

    public class DateServiceImp : DateService
    {
        public DateTime Date
        {
            get
            {
                DateTime date = AppConfigService.AppConfig.IsManual
                    ? AppConfigService.AppConfig.Date
                    : DateTime.Today.AddDays(-1);

                return date;
            }
        }
    }
}