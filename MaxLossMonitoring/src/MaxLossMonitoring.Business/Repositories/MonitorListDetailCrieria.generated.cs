using Fanex.Data.Repository;

namespace MaxLossMonitoring.Business.Repositories
{
    public class MonitorListDetailCrieria : CriteriaBase
    {
        public int ActionId => 2;

        public override string GetSettingKey() => "NAP_LimitLoss_MonitorListDetail";

        public override bool IsValid() => true;
    }
}