using Fanex.Data.Repository;
using System;

namespace Fanex.N88DailyWL.Business.Criterias
{
    public class N88Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_N88";

        public override bool IsValid() => true;
    }
}