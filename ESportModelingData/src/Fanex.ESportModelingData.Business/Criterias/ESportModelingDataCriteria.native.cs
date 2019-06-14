using System;
using Fanex.Data.Repository;

namespace Fanex.ESportModelingData.Business.Criterias
{
    public class ESportModelingDataCriteria : CriteriaBase
    {
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public override string GetSettingKey() => "NAP_DW_Esport_ModelingDataWeekly";

        public override bool IsValid() => true;
    }
}