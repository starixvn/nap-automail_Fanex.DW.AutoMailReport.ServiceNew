using Fanex.Data.Repository;
using System;

namespace Fanex.TopPunterCasino.Business.Criterias
{
    public class CasinoTopPunterCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public int TotalTurnover { get; set; } = 50000;

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_PunterCasino";

        public override bool IsValid() => true;
    }
}