using Fanex.Data.Repository;

namespace Fanex.Letstalk.OneTime.Business.Criterias
{
    public class LetstalkPreRunLogUpdateCriteria : NonQueryCommand
    {
        public override string GetSettingKey() => "Acc_Rpt_LetstalkPreRunLog_Update";

        public override bool IsValid() => true;

        public string FolderSent { get; set; }
    }
}