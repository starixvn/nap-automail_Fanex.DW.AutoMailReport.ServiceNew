using Fanex.Data.Repository;

namespace MaxLossMonitoring.Business.Repositories
{
    public class ActionConfigSelCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_LimitLoss_ActionConfigSel";

        public override bool IsValid() => true;
    }
}