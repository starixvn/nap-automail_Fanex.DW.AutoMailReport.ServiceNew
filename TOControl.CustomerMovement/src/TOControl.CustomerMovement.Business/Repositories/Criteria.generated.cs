using System;
using Fanex.Data.Repository;

namespace TOControl.CustomerMovement.Business.Repositories
{
    public class Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "SpName";

        public override bool IsValid() => true;
    }
}