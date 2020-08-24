using Fanex.Data.Repository;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.Letstalk.OneTime.Business.Criterias
{
    public class CheckMailStatusCriteria : CriteriaBase
    {
        public int Status { get; } = (int)JobStatus.SuccessWithoutSendmail;

        public int SiteId { get; } = -1;

        public string MailName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Mail_Report_GetMailTaskList";

        public override bool IsValid() => true;
    }
}