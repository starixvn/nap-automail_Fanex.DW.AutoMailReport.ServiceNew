using Fanex.Data.Repository;
using System;

namespace Fanex.SoccerNonLive.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_SoccerNonLiveDaily";

        public override bool IsValid() => true;
    }
}