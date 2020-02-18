using Fanex.Data.Repository;
using System;

namespace Fanex.TopLicensee.Service.Criterias
{
    public class TopLicenseeMarginCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_TopLicenseeMargin";

        public override bool IsValid() => true;
    }
}