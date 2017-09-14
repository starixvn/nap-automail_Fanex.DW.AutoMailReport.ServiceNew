using Fanex.Data.Repository;

namespace TOControl.CustomerMovement.Business.Repositories
{
    public class CustomerSelByUserNameCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey() => "Acc_CustomerSelByUserName";

        public override bool IsValid() => true;
    }
}