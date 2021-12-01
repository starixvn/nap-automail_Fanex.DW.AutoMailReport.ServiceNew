using Fanex.Data.Repository;

namespace TopWinningAgent.Business.Repositories
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