using System;
using Fanex.Data.Repository;

namespace BetradarVirtualSportsMonthly.Business.Repositories
{
    public class DailyCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_BetradarVS_GrossWinning_Monthly";

        public override bool IsValid() => true;
    }
}