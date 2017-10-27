using Fanex.Data.Repository;

namespace TOControl.Notification.Business.Repositories
{
    public class CustomerSelByUserNameCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey() => "NAP_EmailNotification_CustomerSel";

        public override bool IsValid() => true;
    }
}