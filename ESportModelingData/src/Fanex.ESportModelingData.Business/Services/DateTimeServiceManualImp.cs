using System;

namespace Fanex.ESportModelingData.Business.Services
{
    public class DateTimeServiceManualImp : DateTimeService
    {
        public DateTime DateEnd
        {
            get
            {
                return AppConfigService.AppConfig.DateEnd;
            }
        }

        public DateTime DateStart
        {
            get
            {
                return AppConfigService.AppConfig.DateStart;
            }
        }
    }
}