using System;
using Fanex.Data.Repository;

namespace Fanex.BetBuilder.Business.Criterias
{
    public class DailyBetBuilderCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Acc_Rpt_Daily_Bet_Builder_AutoMail";
    }
}