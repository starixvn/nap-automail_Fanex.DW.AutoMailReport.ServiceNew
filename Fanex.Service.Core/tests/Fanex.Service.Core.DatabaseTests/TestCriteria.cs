using Fanex.Data.Repository;

namespace Fanex.Service.Core.DatabaseTests
{
    public class TestCriteria : CriteriaBase
    {
        public string Cube => "DW";

        public override string GetSettingKey() => "BI_GetDWStatusByCube";

        public override bool IsValid() => true;
    }
}