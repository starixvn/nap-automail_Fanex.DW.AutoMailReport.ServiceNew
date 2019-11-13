using Fanex.Data.Repository;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class LeagueCritetia : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_DW_Esport_Dim_League";

        public override bool IsValid() => true;
    }
}