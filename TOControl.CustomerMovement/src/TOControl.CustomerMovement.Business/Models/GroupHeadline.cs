using System;

namespace TOControl.CustomerMovement.Business.Models
{
    public class GroupHeadline
    {
        public int Id { get; set; }

        public string OldHeadline { get; set; }

        public string NewHeadline { get; set; }

        public DateTime Date { get; set; }
    }
}