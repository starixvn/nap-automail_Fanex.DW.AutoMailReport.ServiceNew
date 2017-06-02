using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Repositories
{
    public class SportCriteria : CriteriaBase
    {
        public bool Include3rd { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_Automail_BettypeWithProducts_Sport_Select";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}