using Fanex.Data.Repository;

namespace BetradarVirtualSports.Business.Repositories
{
    public class DWStatusByCubeCriteria : CriteriaBase
    {
        public string Cube => "DW";

        public override string GetSettingKey() => "BI_GetDWStatusByCube";

        public override bool IsValid() => true;
    }
}