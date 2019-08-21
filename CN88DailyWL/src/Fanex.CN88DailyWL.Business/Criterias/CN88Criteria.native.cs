using Fanex.Data.Repository;
using System;

namespace Fanex.CN88DailyWL.Business.Criterias
{
    public class CN88Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_CN88WinLoss";

        public override bool IsValid() => true;
    }
}