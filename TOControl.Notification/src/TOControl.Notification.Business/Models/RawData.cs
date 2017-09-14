using System;
using System.Collections.Generic;

namespace TOControl.Notification.Business.Models
{
    public class RawData
    {
        public int Id { get; set; }

        public string NotificationInfo { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ActionId { get; set; }

        public string Action
        {
            get
            {
                var actions = new Dictionary<int, string>
                {
                    { 1000, "Add" },
                    { 1002, "Remove" }
                };

                if (!actions.ContainsKey(ActionId))
                {
                    return string.Empty;
                }

                return actions[ActionId];
            }
        }
    }
}