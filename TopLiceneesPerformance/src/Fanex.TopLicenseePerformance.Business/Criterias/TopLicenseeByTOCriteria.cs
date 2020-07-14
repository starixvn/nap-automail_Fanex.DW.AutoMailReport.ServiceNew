using Fanex.Data.Repository;
using System;

namespace Fanex.TopLicensee.Service.Criterias
{
    public class TopLicenseePerformanceCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public int SportType { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_TopLicenseePerfByTO";

        public override bool IsValid() => true;
    }
}