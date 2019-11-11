using Fanex.Data.Repository;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class DateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_DW_Esport_Dim_Date";

        public override bool IsValid() => true;
    }
}