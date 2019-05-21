using System;
using Fanex.Data.Repository;

namespace Fanex.ServiceMonitoring.Criterias
{
    public class MailReportCriteria : CriteriaBase
    {
        public string MailName { get; set; }

        public int SiteId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Status { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
        {
            return "Mail_Report_GetMailTaskList";
        }
    }
}