using Fanex.Data.Repository;

namespace Fanex.SoccerPerformance.Business.Criterias
{
    public class GetDWStatusByCubeCriteria : CriteriaBase
    {
        public string Cube => "DW";

        public override string GetSettingKey()
        {
            return "BI_GetDWStatusByCube";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
