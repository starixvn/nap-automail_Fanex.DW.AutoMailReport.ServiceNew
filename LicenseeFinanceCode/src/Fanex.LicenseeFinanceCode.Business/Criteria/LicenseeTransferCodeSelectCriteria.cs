using Fanex.Data.Repository;
using System;

namespace Fanex.LicenseeFinanceCode.Business.Criteria
{
    public class LicenseeTransferCodeSelectCriteria : CriteriaBase
    {
        public string LicenseeIDs { get; } = "All";

        public bool ForEmail { get; } = true;

        public int StatusIds { get; } = 3;

        public DateTime MoveBetsDay { get; set; }

        public override string GetSettingKey() => "Acc_LicenseeTransferCode_Sel";

        public override bool IsValid() => true;
    }
}