using Fanex.Data.Repository;
using System;

namespace Fanex.TopSportbookAgents.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public int Winloss { get; set; } = 100000;

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_SportbookTopAgents";

        public override bool IsValid() => true;
    }
}