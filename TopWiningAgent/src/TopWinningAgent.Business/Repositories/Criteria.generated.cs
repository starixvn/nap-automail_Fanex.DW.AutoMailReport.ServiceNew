using System;
using Fanex.Data.Repository;

namespace TopWinningAgent.Business.Repositories
{
    public class Criteria : CriteriaBase
    {
        public DateTime ? DateServiceRun { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_TopWinningAgent_Weekly";

        public override bool IsValid() => true;
    }
}