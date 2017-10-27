using System;

namespace TOControl.Notification.Business.Models
{
    public class CustomerMovement
    {
        public int Id { get; set; }

        public string GroupHeadline { get; set; }

        public string GroupMember { get; set; }

        public string Action { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }

        public int Currency { get; set; }
    }
}