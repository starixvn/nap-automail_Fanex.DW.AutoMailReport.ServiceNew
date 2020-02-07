using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.MailLetstalkTemplate.Business.Criterias
{
    public class Criteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(Ignored = true)]
        public string SettingKey { get; set; }

        public override string GetSettingKey() => SettingKey;

        public override bool IsValid() => true;
    }
}