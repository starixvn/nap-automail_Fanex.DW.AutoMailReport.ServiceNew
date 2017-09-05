using System;
using Fanex.Data.Repository;

namespace EventMarketOffer.Business.Repositories
{
    public class CriteriaEventMartketOffer : CriteriaBase
    {
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public override string GetSettingKey() => "Acc_Rpt_Automail_Monthly_Event_Market_Offer";

        public override bool IsValid() => true;
    }
}