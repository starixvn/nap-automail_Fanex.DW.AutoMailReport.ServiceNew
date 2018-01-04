using Fanex.Data.Repository;

namespace NegativeBalance.Business.Repositories
{
    public class SelectMailingListCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "OIC_LicenseeMailingListSel";

        public override bool IsValid() => true;
    }
}