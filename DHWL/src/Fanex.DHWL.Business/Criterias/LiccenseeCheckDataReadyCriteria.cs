using Fanex.Data.Repository;

namespace Fanex.DHWL.Business.Criterias
{
    public class LiccenseeCheckDataReadyCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Task_rptBetting_CheckDataReady";

        public override bool IsValid() => true;
    }
}