using Fanex.Data.Repository;
using System;

namespace Fanex.BigCreditIncrement789Y.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_BigCreditIncrement";

        public override bool IsValid() => true;
    }
}