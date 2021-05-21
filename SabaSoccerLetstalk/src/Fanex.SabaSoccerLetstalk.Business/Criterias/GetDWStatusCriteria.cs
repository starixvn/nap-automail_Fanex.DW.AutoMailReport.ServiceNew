using Fanex.Data.Repository;

namespace Fanex.SabaSoccerLetstalk.Business.Criterias
{
    public class GetDWStatusCriteria : CriteriaBase
    {
        public string TaskIDList => "16";

        public override string GetSettingKey() => "DailyJob_CheckTasksStatus";

        public override bool IsValid() => true;
    }
}