using Fanex.Data.Repository;

namespace Fanex.SuspiciousAccount.Business.Criterias
{
    public class SelectAccountCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Acc_SuspiciousAccountLetstalk_Select";

        public override bool IsValid() => true;
    }
}