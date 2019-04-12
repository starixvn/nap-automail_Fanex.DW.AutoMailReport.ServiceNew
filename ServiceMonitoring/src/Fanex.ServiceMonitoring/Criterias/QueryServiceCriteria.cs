using Fanex.Data.Repository;

namespace Fanex.ServiceMonitoring.Criterias 
{
    public class QueryServiceCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "ServiceMonitoring_Select";

        public override bool IsValid() => true;
    }
}