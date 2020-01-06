using Fanex.Data.Repository;
using System;

namespace Fanex.IBOPerformance.Service.Criterias
{
    public class DailyCreatedCriteria : CriteriaBase
    {
        public string Site { get; set; } = "IBO";

        public int CurrencyId { get; set; } = 13;

        public DateTime Yesterday { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_AllMembersForIBO_Daily";

        public override bool IsValid() => true;
    }
}