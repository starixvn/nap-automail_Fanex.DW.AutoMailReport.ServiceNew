using Fanex.Data.Repository;

namespace Fanex.N88DailyWL.Business.Criterias
{
    public class GetDWStatusByCubeCriteria : CriteriaBase
    {
        public string Cube => "DW";

        public override string GetSettingKey() => "BI_GetDWStatusByCube";

        public override bool IsValid() => true;
    }
}