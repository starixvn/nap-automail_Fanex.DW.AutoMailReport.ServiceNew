using System.Collections.Generic;

namespace TOControl.Notification.Business.Models
{
    public class CustomerInfomation
    {
        private readonly Dictionary<int, string> roles;

        public CustomerInfomation()
        {
            roles = new Dictionary<int, string>
            {
                { 1, "Member" },
                { 2, "Agent" },
                { 3, "Master" },
                { 4, "Super" }
            };
        }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public string Level
        {
            get
            {
                if (!roles.ContainsKey(RoleId))
                {
                    return string.Empty;
                }

                return roles[RoleId];
            }
        }
    }
}