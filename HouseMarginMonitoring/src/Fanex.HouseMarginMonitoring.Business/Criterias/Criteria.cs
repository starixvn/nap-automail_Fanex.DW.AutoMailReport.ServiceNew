using Fanex.Data.Repository;
using System;

namespace Fanex.HouseMarginMonitoring.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal? WLThreshold { get; set; } = -10000;

        public decimal? MarginThreshold { get; set; } = -10;

        public override string GetSettingKey() => "Acc_Rpt_Letstalk_HouseMarginMonitor";

        public override bool IsValid() => true;
    }
}