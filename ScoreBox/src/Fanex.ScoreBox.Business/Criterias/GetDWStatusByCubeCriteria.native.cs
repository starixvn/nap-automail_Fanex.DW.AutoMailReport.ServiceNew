using Fanex.Data.Repository;

namespace Fanex.ScoreBox.Business.Criterias
{
    public class GetDWStatusByCubeCriteria : CriteriaBase
    {
        public string Cube => "DW";

        public override string GetSettingKey() => "BI_GetDWStatusByCube";

        public override bool IsValid() => true;        
    }
}