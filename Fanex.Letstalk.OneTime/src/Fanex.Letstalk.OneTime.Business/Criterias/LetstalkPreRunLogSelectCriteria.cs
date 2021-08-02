using Fanex.Data.Repository;
using System;

namespace Fanex.Letstalk.OneTime.Business.Criterias
{
    public class LetstalkPreRunLogSelectCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Acc_Rpt_LetstalkPreRunLog_Select";

        public override bool IsValid() => true;

        public string Date { get; set; }

        public bool IsSent { get; set; }
    }
}