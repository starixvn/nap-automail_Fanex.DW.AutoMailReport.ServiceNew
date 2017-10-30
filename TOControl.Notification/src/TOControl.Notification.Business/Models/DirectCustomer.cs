using System;

namespace TOControl.Notification.Business.Models
{
    public class DirectCustomer
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Level { get; set; }

        public DateTime Date { get; set; }

        public int Currency { get; set; }
    }
}