using Fanex.Data.Repository;

namespace TOControl.CustomerMovement.Business.Repositories
{
    public class GroupHeadlineSelectByGroupIdCrietria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override string GetSettingKey() => "Acc_GroupHeadlineSelectByGroupID";

        public override bool IsValid() => true;
    }
}