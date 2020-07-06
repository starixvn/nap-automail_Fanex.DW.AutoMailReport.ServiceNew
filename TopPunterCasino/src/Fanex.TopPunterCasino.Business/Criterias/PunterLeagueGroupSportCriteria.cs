using Fanex.Data.Repository;
using System;

namespace Fanex.SabaSoccerLetstalk.Business.Criterias
{
    public class PunterLeagueGroupSportCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public string LeagueGroup { get; set; } = "42,74";

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_PunterLeagueGroupSport";

        public override bool IsValid() => true;
    }
}