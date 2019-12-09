using Fanex.Data.Repository;
using System;

namespace Fanex.AbnormalBets.Business.Criterias
{
    public class AbnormalDailyCSDCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_AbnormalBetAutoMail_Daily_For_CSD";

        public override bool IsValid() => true;
    }
}