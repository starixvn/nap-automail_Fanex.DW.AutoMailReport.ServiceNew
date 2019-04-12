using System;

namespace Fanex.ServiceMonitoring.Models
{
    public class MailTaskDetailModel
    {
        public int ScheduleID { get; set; }

        public int RepeatType { get; set; }

        public string CronExpression { get; set; }

        public DateTime StartOn { get; set; }

        public DateTime? EndOn { get; set; }

        public int Interval { get; set; }

        public int Attempt { get; set; }

        public int MailId { get; set; }

        public string MailName { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public DateTime? LastSent { get; set; }

        public DateTime LastScheduledTime { get; set; }
    }
}