using System;

namespace Fanex.BigCreditIncrement789Y.Business.Models
{
    public class PunterModel
    {
        public string UserName { get; set; }

        public DateTime Creation { get; set; }

        public string Currency { get; set; }

        public decimal OldCredit { get; set; }

        public decimal NewCredit { get; set; }

        public decimal OldRMCredit { get; set; }

        public decimal NewRMCredit { get; set; }
    }
}