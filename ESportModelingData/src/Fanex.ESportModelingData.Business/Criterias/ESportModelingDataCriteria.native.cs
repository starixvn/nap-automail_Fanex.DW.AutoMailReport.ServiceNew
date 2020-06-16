using Fanex.Data.Repository;
using System;

namespace Fanex.ESportModelingData.Business.Criterias
{
    public class ESportModelingDataCriteria : CriteriaBase
    {
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public override string GetSettingKey() => "NAP_DW_ModelingDataWeekly";

        public override bool IsValid() => true;
    }
}