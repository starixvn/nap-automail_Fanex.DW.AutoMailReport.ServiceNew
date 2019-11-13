using Fanex.Data.Repository;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class CurrencyCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_DW_Esport_Dim_Currency";

        public override bool IsValid() => true;
    }
}