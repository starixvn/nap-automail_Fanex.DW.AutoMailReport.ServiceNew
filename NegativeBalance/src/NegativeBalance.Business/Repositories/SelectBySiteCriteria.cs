using Fanex.Data.Repository;

namespace NegativeBalance.Business.Repositories
{
    public class SelectBySiteCriteria : CriteriaBase
    {
        public string Site { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey() => "Acc_BalanceNegativeSel_BySite";

        public override bool IsValid() => true;
    }
}