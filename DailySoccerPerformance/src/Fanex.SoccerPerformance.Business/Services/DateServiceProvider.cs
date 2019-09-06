using System.Collections.Generic;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface DateServiceProvider
    {
        DateService Date { get; }
    }

    public class DateServiceProviderImp : DateServiceProvider
    {
        public DateService Date
        {
            get
            {
                var dates = new Dictionary<bool, DateService>
                {
                    { true, new DateServiceManualImp() },
                    { false , new DateServiceAutoImp() },
                };

                return dates[AppConfigService.AppConfig.IsManual];
            }
        }
    }
}
