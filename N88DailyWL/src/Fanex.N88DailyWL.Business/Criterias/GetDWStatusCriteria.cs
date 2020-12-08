using Fanex.Data.Repository;

namespace Fanex.N88DailyWL.Business.Criterias
{
    public class GetDWStatusCriteria : CriteriaBase
    {
        public string TaskIDList => "21";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid() => true;
    }
}