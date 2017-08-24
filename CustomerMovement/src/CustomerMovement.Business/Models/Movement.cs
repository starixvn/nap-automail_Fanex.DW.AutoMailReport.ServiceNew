using System;

namespace CustomerMovement.Business.Models
{
    public class Movement
    {
        public string GroupHeadline { get; set; }

        public string GroupMember { get; set; }

        public string Action { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }
    }
}