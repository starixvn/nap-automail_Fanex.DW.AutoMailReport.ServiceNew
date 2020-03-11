using Fanex.Data.Repository;
using System;

namespace Fanex.AbnormalBets.Business.Criterias
{
    public class AbnormalHeadlineCriteria : CriteriaBase
    {
        public string GroupName { get; set; }

        public DateTime RptDate { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_AbnormalBetAutoMail_Daily_For_Headline";

        public override bool IsValid() => true;
    }
}