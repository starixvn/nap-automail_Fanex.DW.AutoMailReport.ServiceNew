using Fanex.Data.Repository;

namespace Fanex.DHWL.Business.Criterias
{
    public class GetDWStatusCriteria : CriteriaBase
    {
        public string TaskIDList => "6,11,17,52";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}