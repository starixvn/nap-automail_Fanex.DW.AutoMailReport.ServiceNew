using Fanex.Data.Repository;
using System;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class MatchCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "NAP_DW_Esport_Dim_Match";

        public override bool IsValid() => true;
    }
}