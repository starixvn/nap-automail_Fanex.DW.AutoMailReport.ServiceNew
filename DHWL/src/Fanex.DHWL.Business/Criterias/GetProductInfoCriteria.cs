using Fanex.Data.Repository;

namespace Fanex.DHWL.Business.Criterias
{
    public class GetProductInfoCriteria : CriteriaBase
    {
        public int ProductID { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_HouseNetIncome_GetProductInfo";

        public override bool IsValid() => true;
    }
}