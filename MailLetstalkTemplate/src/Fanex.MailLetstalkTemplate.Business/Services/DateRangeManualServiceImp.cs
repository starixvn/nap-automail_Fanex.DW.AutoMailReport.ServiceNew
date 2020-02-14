using System;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public class DateRangeManualServiceImp : DateRangeService
    {
        public DateTime FromDate
        {
            get
            {
                return AppConfigService.Config.FromDate;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return AppConfigService.Config.ToDate;
            }
        }
    }
}