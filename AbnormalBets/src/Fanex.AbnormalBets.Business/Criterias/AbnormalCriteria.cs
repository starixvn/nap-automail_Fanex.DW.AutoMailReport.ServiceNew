using Fanex.Data.Repository;
using System;

namespace Fanex.AbnormalBets.Business.Criterias
{
    public class AbnormalCriteria : CriteriaBase
    {
        public string Site { get; set; }

        public DateTime RptDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_AbnormalBetAutoMail";

        public override bool IsValid() => true;
    }
}