using Fanex.Data.Repository;

namespace NegativeBalance.Business.Repositories
{
    public class SelectMailingListCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Acc_MailingListSel";

        public override bool IsValid() => true;
    }
}