using Fanex.Data;
using Fanex.Data.Repository;

namespace TOControl.Notification.Business.Repositories
{
    public class EmailNotificationGlobalSettingSelectCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "NotifiedByFunction_Flag")]
        public int NotifiedByFunctionFlag { get; set; }

        public override string GetSettingKey() => "NAP_EmailNotification_GlobalSettingSelect";

        public override bool IsValid() => true;
    }
}