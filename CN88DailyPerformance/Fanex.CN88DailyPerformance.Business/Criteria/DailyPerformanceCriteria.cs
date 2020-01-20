using Fanex.Data.Repository;
using System;

namespace Fanex.CN88DailyPerformance.Service.Criteria
{
    public class DailyPerformanceCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_CN88Daily";

        public override bool IsValid() => FromDate <= ToDate;
    }
}
