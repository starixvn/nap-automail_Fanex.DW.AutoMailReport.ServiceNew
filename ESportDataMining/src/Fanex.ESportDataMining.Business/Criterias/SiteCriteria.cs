using Fanex.Data.Repository;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class SiteCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_DW_Esport_Dim_Site";

        public override bool IsValid() => true;
    }
}