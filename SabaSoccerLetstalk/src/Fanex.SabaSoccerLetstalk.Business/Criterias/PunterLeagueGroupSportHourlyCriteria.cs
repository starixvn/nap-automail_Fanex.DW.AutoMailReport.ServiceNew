using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.SabaSoccerLetstalk.Business.Criterias
{
    public class PunterLeagueGroupSportHourlyCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        [SpParam(Ignored = true)]
        public string SPName { get; set; }

        public override string GetSettingKey() => SPName;

        public override bool IsValid() => true;
    }
}