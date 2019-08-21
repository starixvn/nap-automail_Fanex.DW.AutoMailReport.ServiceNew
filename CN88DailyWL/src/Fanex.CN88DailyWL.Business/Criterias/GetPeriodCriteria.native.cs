using Fanex.Data.Repository;
using System;

namespace Fanex.CN88DailyWL.Business.Criterias
{
    public class GetPeriodCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_GetAccountingPeriod";

        public override bool IsValid() => true;
    }
}