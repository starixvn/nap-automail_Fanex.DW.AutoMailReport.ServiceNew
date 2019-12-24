using Fanex.Data.Repository;
using System;

namespace Fanex.AbnormalBets.Business.Criterias
{
    public class CheckMailStatusCriteria : CriteriaBase
    {
        public int Status { get; } = 4;

        public int SiteId { get; } = -1;

        public string MailName { get; } = "Abnormal Bet Daily Report";

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Mail_Report_GetMailTaskList";

        public override bool IsValid() => true;
    }
}