using Fanex.Data.Repository;

namespace Fanex.DHWL.Business.Criterias
{
    public class GetDWStatusCriteria : CriteriaBase
    {
        public string TaskIDList => "17,44";

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