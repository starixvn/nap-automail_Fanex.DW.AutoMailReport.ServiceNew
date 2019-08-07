using Fanex.Data.Repository;
using System;

namespace Fanex.ScoreBox.Business.Criterias
{
    public class ScoreBoxCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_DailyScoreBox";

        public override bool IsValid() => true;
    }
}