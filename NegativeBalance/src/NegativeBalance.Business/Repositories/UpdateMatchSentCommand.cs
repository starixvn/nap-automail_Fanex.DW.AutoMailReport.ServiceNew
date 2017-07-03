using Fanex.Data;
using Fanex.Data.Repository;

namespace NegativeBalance.Business.Repositories
{
    public class UpdateMatchSentCommand : NonQueryCommand
    {
        public string LogIds { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey() => "Acc_BalanceNegativeUpd_StatusID";

        public override bool IsValid() => true;
    }
}