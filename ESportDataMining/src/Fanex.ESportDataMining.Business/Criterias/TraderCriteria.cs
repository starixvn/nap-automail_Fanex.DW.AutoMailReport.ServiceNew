using Fanex.Data.Repository;
using System;

namespace Fanex.ESportDataMining.Business.Criterias
{
    public class TraderCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "NAP_DW_Esport_Dim_Trader";

        public override bool IsValid() => true;
    }
}