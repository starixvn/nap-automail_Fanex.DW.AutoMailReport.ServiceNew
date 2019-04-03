using System;

namespace Fanex.ServiceMonitoring.Models
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public int Interval { get; set; }

        public bool IsActive { get; set; }

        public int OrderBy { get; set; }

        public string IPServer { get; set; }

        public DateTime LastUpdate { get; set; }

        public int Diff { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsRestartSuccessfully { get; set; }
    }
}