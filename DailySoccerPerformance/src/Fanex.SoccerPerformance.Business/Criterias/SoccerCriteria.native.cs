using Fanex.Data.Repository;
using System;

namespace Fanex.SoccerPerformance.Business.Criterias
{
    public class SoccerCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Daily_Soccer_Performance";

        public override bool IsValid() => true;
    }
}
