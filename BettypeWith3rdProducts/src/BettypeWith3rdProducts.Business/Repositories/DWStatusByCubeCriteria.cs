using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Repositories
{
    public class DWStatusByCubeCriteria : CriteriaBase
    {
#pragma warning disable S2325 // static property
        public string Cube => "DW";
#pragma warning restore S2325 // static property

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