using Fanex.Data.Repository;
using System;

namespace Fanex.TopLicenseePerformance.Business.Criterias
{
    public class TopLeagueGroupSportLicenseePerfCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string LeagueGroup { get; set; }

        public int SportType { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_LeagueGroupSport_LicenseePerf";

        public override bool IsValid() => true;
    }
}