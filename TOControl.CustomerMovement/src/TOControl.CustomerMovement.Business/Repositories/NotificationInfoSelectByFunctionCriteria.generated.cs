using System;
using Fanex.Data;
using Fanex.Data.Repository;

namespace TOControl.CustomerMovement.Business.Repositories
{
    public class NotificationInfoSelectByFunctionCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ActionIDs { get; set; }

        [SpParam(SqlParamName = "NotifiedByFunction_Flag")]
        public int NotifiedByFunctionFlag { get; set; }

        public override string GetSettingKey() => "NAP_Email_NotificationInfoSelectByFunction";

        public override bool IsValid() => true;
    }
}