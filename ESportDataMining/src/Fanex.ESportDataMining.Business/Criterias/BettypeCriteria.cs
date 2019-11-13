using Fanex.Data.Repository;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class BettypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_DW_Esport_Dim_Bettype";

        public override bool IsValid() => true;
    }
}