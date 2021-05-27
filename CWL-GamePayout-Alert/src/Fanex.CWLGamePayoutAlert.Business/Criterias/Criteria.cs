using Fanex.Data.Repository;
using System;

namespace Fanex.CWLGamePayoutAlert.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime FromGMT8Time { get; set; }

        public int TimeInterval { get; set; }

        public decimal TLV { get; set; }

        public decimal MWinLV { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_NonSports_PayoutAlert";

        public override bool IsValid() => true;
    }
}