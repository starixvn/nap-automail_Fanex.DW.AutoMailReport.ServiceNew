using Fanex.Data;
using Fanex.Data.Repository;

namespace TOControl.Notification.Business.Repositories
{
    public class NotificationInfoUpdateByFunctionCommand : NonQueryCommand
    {
        public string NotificationIdList { get; set; }

        [SpParam(SqlParamName = "NotifiedByFunction_Flag")]
        public int NotifiedByFunctionFlag { get; set; }

        public int IsNotifiedValue { get; set; }

        public override string GetSettingKey() => "NAP_Email_NotificationInfoUpdateByFunction";

        public override bool IsValid() => true;
    }
}