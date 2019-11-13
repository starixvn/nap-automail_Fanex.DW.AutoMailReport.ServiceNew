using Fanex.Data.Repository;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class LiveStatusCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_DW_Esport_Dim_LiveStatus";

        public override bool IsValid() => true;
    }
}