using Fanex.Data.Repository;
using System;

namespace Fanex.SBABICustomerPerformance.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Daily_Top_Winning";

        public override bool IsValid() => true;
    }
}