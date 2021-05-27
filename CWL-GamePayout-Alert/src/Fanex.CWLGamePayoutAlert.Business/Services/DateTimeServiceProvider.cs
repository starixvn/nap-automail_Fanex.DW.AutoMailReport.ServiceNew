using System.Collections.Generic;

namespace Fanex.CWLGamePayoutAlert.Business.Services
{
    public interface DateTimeServiceProvider
    {
        DateTimeService DateTime { get; }
    }

    public class DateTimeServiceProviderImp : DateTimeServiceProvider
    {
        private Dictionary<bool, DateTimeService> Date
        {
            get
            {
                return new Dictionary<bool, DateTimeService>
                {
                    { true, new DateTimeServiceManualImp()},
                    { false, new DateTimeServiceImp()},
                };
            }
        }

        public DateTimeService DateTime
        {
            get
            {
                return Date[AppConfigService.Config.IsManual];
            }
        }
    }
}