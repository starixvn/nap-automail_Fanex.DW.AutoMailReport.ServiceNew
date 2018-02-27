using Fanex.Data.Repository;

namespace TopWinningAgent.Business.Repositories
{
    public class Criteria : CriteriaBase
    {
        public override string GetSettingKey() => "Acc_Rpt_Automail_TopWinningAgent_Weekly";

        public override bool IsValid() => true;
    }
}