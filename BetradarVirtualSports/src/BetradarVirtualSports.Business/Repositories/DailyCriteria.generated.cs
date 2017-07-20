using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace BetradarVirtualSports.Business.Repositories
{
    public class DailyCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "rptdate")]
        public DateTime Date { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_BetradarVS_GrossWinning_Daily";

        public override bool IsValid() => true;
    }
}