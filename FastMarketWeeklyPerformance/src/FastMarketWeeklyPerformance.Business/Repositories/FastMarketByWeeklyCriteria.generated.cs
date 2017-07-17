using System;
using Fanex.Data.Repository;

namespace FastMarketWeeklyPerformance.Business.Repositories
{
    public class FastMarketByWeeklyCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_FastMarketByWeekly";

        public override bool IsValid() => true;
    }
}