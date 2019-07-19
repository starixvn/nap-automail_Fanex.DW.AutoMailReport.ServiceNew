using Fanex.Data.Repository;
using System;

namespace Fanex.FinanceTransfer.Business.Criterias
{
    public class TransferCriteria : NonQueryCommand
    {
        public DateTime MoveBetsDay { get; set; }

        public DateTime? TransferMonthDate { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Acc_LicenseeTransfer_Job";

        public override bool IsValid() => true;
    }
}