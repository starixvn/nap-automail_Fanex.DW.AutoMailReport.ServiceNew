using Fanex.Data.Repository;
using System;

namespace Fanex.TopPunterCasino.Business.Criterias
{
    public class PunterNonSportCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public int TotalTurnover { get; set; } = 50000;

        public string ProductIds { get; set; } = "8,28,40,50,5,27,3";

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_PunterNonSport";

        public override bool IsValid() => true;
    }
}