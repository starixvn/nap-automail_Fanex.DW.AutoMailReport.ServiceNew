using Fanex.Data.Repository;

namespace MaxLossMonitoring.Business.Repositories
{
    public class MonitorListCriteria : CriteriaBase
    {
        public int ActionId => 3;

        public override string GetSettingKey() => "NAP_LimitLoss_MonitorList";

        public override bool IsValid() => true;
    }
}