using Fanex.Data.Repository;

namespace Fanex.SuspiciousAccount.Business.Criterias
{
    public class UpdateIsNotifiedAccount : NonQueryCommand
    {
        public string MemNotify { get; set; }

        public override string GetSettingKey() => "Acc_SuspiciousAccountLetstalk_Update";

        public override bool IsValid() => true;
    }
}