using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Repositories
{
    public class GetDWStatusCriteria : CriteriaBase
    {
        public string TaskIDList => "6,8,9,10";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid() => true;
    }
}