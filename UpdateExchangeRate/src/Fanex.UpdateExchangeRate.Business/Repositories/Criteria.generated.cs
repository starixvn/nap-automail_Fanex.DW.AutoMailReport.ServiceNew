using Fanex.Data.Repository;

namespace Fanex.UpdateExchangeRate.Business.Repositories
{
    public class Criteria : CriteriaBase
    {
        public override string GetSettingKey() => "Acc_rpt_Weekly_CurrencyUpdatedRate";

        public override bool IsValid() => true;
    }
}