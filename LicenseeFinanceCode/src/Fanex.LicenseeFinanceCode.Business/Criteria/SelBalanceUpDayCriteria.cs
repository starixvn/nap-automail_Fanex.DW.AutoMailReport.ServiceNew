using Fanex.Data.Repository;

namespace Fanex.LicenseeFinanceCode.Business.Criteria
{
    public class SelBalanceUpDayCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "AGE_SelBalanceUpday";

        public override bool IsValid() => true;
    }
}