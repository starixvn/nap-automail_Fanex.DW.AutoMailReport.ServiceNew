using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Repositories
{
    public class DWStatusByCubeCriteria : CriteriaBase
    {
        public string Cube
        {
            get { return "DW"; }
        }

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