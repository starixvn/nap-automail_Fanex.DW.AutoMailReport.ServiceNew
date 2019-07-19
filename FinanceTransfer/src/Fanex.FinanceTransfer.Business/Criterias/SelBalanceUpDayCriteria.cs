using Fanex.Data.Repository;

namespace Fanex.FinanceTransfer.Business.Criterias
{
    public class SelBalanceUpDayCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "AGE_SelBalanceUpDay";

        public override bool IsValid() => true;
    }
}