using Fanex.Data.Repository;

namespace NegativeBalance.Business.Repositories
{
    public class SelectMatchLogCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public int GroupId { get; set; }

        public override string GetSettingKey() => "Acc_BalanceNegativeSel_MatchLog";

        public override bool IsValid() => true;
    }
}