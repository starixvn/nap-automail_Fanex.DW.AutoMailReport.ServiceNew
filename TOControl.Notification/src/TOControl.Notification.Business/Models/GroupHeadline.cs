using System;

namespace TOControl.Notification.Business.Models
{
    public class GroupHeadline
    {
        public int Id { get; set; }

        public string OldHeadline { get; set; }

        public string NewHeadline { get; set; }

        public DateTime Date { get; set; }

        public int Currency { get; set; }
    }
}