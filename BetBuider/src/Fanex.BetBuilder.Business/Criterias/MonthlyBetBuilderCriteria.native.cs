using System;
using Fanex.Data.Repository;

namespace Fanex.BetBuilder.Business.Criterias
{
    public class MonthlyBetBuilderCriteria : CriteriaBase
    {
        public DateTime RptDate { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Acc_Rpt_Monthly_Bet_Builder_AutoMail";
    }
}