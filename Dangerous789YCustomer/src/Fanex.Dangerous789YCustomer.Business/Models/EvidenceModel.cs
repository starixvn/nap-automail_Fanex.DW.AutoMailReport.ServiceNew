using System;

namespace Fanex.Dangerous789YCustomer.Business.Models
{
    public class EvidenceModel
    {
        public int CustId { get; set; }

        public int Level0CustId { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int AssociatedLevel { get; set; }
    }
}