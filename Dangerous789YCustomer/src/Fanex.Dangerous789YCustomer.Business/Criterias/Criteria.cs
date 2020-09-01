using Fanex.Data.Repository;
using System;

namespace Fanex.Dangerous789YCustomer.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime CheckDayGMTP8 { get; set; }

        public int Threshold { get; set; } = 30000;

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_DangerousCustomersTracking";

        public override bool IsValid() => true;
    }
}