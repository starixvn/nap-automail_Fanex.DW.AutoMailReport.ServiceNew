using System;

namespace TOControl.Notification.Business.Models
{
    public class AppConfig
    {
        public int PeriodDate { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public bool IsManual { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsRealCurrency { get; set; }
    }
}