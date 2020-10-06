using Fanex.Data.Repository;

namespace BetradarVirtualSportsDaily.Business.Repositories
{
    public class DWStatusByCubeCriteria : CriteriaBase
    {
        public string TaskIDList => "38";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid() => true;
    }
}