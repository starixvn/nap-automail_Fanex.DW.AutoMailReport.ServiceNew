using System;

namespace EventMarketOffer.Business.Models
{
    public class AppConfig
    {
        public string ExportDataDirectory { get; set; }

        public string ExcelPasswordProtected { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public int FunctionIdNodata { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public bool IsManual { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public Uri NapReportUrl { get; set; }
    }
}