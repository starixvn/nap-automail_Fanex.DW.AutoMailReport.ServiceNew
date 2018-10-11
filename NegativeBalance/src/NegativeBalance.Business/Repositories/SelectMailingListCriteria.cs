using Fanex.Data.Repository;

namespace NegativeBalance.Business.Repositories
{
    public class SelectMailingListCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "BetDeletionReport_MailingList_Select";

        public override bool IsValid() => true;
    }
}