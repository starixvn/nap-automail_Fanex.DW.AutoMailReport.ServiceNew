using System;

namespace TOControl.Notification.Business.Models
{
    public class CustomerMovement
    {
        public int Id { get; set; }

        public string GroupHeadline { get; set; } = string.Empty;

        public string GroupMember { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int Count { get; set; }

        public int Currency { get; set; }
    }
}